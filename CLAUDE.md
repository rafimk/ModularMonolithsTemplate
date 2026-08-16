# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A .NET 10 modular monolith **template**, based on the "Evently" reference architecture (remnants of the
original name remain in a few places: `Evently.Api.http`, `EventlyException.cs`, Serilog app name
`Evently.Api`, Swagger title). The `Users` module is the only fully scaffolded module and acts as the
reference implementation to copy when adding a new module. Solution/company naming uses `CompanyName.*`
as a single, consistently-cased placeholder token throughout the repo (namespaces, file/folder names,
Docker service/container names, appsettings hostnames, the Keycloak realm) — the only deliberate exception
is the Docker image tag (`companynameapi`), which is lowercased because Docker repository names must be.

This repo is also a `dotnet new` template (`.template.config/template.json`, `sourceName: "CompanyName"`).
Install it with `dotnet new install .` from the repo root, then scaffold an instance with
`dotnet new modular-monolith -n YourCompanyName -o path/to/output` — this renames every `CompanyName.*`
occurrence (files, folders, namespaces, Docker/Keycloak config), regenerates the API's `UserSecretsId`, and
lowercases the Docker image tag automatically. `.claude/`, `bin/`, `obj/`, `.vs/`, `.git/` and
`.containers/` are excluded from the generated output.

Local dev stack: `docker compose up` builds the API image and starts Postgres, Keycloak (imports the
`CompanyName` realm from `.files/CompanyName-realm-export.json` on first boot), Seq, Redis, and Jaeger. The
HTTPS dev cert must be exported once per machine before the API container can bind 8081 — Visual Studio does
this automatically on F5, but from the CLI you need to run it yourself:
`dotnet dev-certs https -ep %APPDATA%\ASP.NET\Https\CompanyName.Api.pfx -p <password>` then
`dotnet user-secrets set "Kestrel:Certificates:Default:Password" "<password>" --project src/API/CompanyName.Api/CompanyName.Api`.

## Known current-state issues

`dotnet build CompanyName.slnx` succeeds end-to-end from a warm NuGet cache (only NU1701/MSB3277 warnings
remain, from `Microsoft.EntityFrameworkCore.Tools` pulling a slightly different EF patch version transitively
in `IntegrationTests` — harmless, not worth chasing). From a **cold** cache/clean clone, restoring
`CompanyName.Modules.Users.IntegrationTests` currently fails with `NU1903` (`SSH.NET 2025.1.0` has a
disclosed high-severity advisory, pulled in transitively via `Testcontainers`) — `TreatWarningsAsErrors` in
`Directory.Build.props` turns that NuGet-audit warning into a hard build error. Not yet fixed; needs either
a `SSH.NET`/`Testcontainers` version bump or an explicit audit suppression, someone's call to make.

Two things still open in `CompanyName.Modules.Users.ArchitectureTests`/`IntegrationTests`:

- `ArchitectureTests` is an empty stub project — no NetArchTest rules written yet, despite
  `NetArchTest.Rules` being wired up and ready to use.
- `IntegrationTests` has an initial EF Core migration to build against now (`Database/Migrations/`,
  previously missing entirely — the schema was never created), but still has no actual integration tests
  written, despite `Testcontainers.*` being wired up and ready to use.

`AnalysisMode` in `Directory.Build.props` is `Default` (not `All`) — it was dialed back deliberately because
`All` turns on Design/Naming/Performance analyzer categories that fight this architecture's own patterns
(CQRS marker interfaces, DI-instantiated pipeline behaviors, no `ConfigureAwait` needed in ASP.NET Core).
`.editorconfig` carries a documented list of further per-rule suppressions (`dotnet_diagnostic.CAxxxx.severity
= none`) for genuine conflicts between analyzers and intentional design (e.g. `Result<T>`'s implicit
conversion, `User.Roles` needing to stay a property for EF's navigation mapping). Don't remove these without
checking why they're there — each has a one-line justification comment.

## Commands

```bash
dotnet build CompanyName.slnx
dotnet test src/Modules/Users/CompanyName.Modules.Users.UnitTests/CompanyName.Modules.Users.UnitTests.csproj
dotnet test --filter "FullyQualifiedName~UserTests.MethodName"   # single test
```

Scaffolding a new module follows the pattern in `README.md` — eight class libraries per module
(`Domain`, `Application`, `ArchitectureTests`, `Infrastructure`, `IntegrationEvents`, `IntegrationTests`,
`Presentation`, `UnitTests`), e.g.:

```bash
dotnet new classlib -n CompanyName.Modules.<Name>.Domain -o src/Modules/<Name>/CompanyName.Modules.<Name>.Domain -f net10.0
```

NuGet package versions are centrally managed (`Directory.Packages.props`) — add a `<PackageVersion>` there
before adding a `<PackageReference>` in any `.csproj`.

## Architecture

**Layering per module** (mirrors Domain-Driven Design / Clean Architecture, enforced conceptually by the
per-module `ArchitectureTests` project using NetArchTest): `Domain` → `Application` → `Infrastructure` /
`Presentation`, plus two boundary-crossing projects:
- `IntegrationEvents` — the only project other modules are allowed to reference; holds public events a
  module publishes for others to consume (e.g. `UserRegisteredIntegrationEvent`).
- `Infrastructure` exposes a single `<Module>Module.Add<Module>Module(IServiceCollection, IConfiguration)`
  entry point (see `UsersModule.cs`) that wires EF Core, repositories, outbox/inbox jobs, and endpoint
  registration for that module. `Program.cs` in the API project calls one of these per module — this is
  the seam for adding a new module to the host.

**Shared kernel** lives under `src/Common/*` and is referenced by every module:
- `Common.Domain` — `Entity` (domain-event-raising base class), `Result`/`Result<T>` (railway-oriented
  outcome type used instead of exceptions for expected failures), `Error`/`ErrorType`/`ValidationError`.
- `Common.Application` — MediatR `ICommand`/`IQuery` + handler interfaces, and MediatR pipeline behaviors
  (`ValidationPipelineBehavior` runs FluentValidation validators, `ExceptionHandlingPipelineBehavior`,
  `RequestLoggingPipelineBehavior`).
- `Common.Infrastructure` — cross-cutting registration (`InfrastructureConfiguration.AddInfrastructure`):
  auth, Quartz, Redis distributed cache, MassTransit (in-memory transport), OpenTelemetry, the outbox
  interceptor.
- `Common.Presentation` — minimal-API endpoint plumbing: implement `IEndpoint` per endpoint class,
  `EndpointExtensions.AddEndpoints`/`MapEndpoints` discover and register them via reflection over each
  module's `Presentation` assembly. `ApiResults`/`ResultExtensions` translate a `Result` into an HTTP
  response (`Result.Match(Results.Ok, ApiResults.Problem)`).

**Request flow** (see `RegisterUser.cs` → `RegisterUserCommandHandler.cs`): minimal-API endpoint maps an
HTTP request to a MediatR command/query → handler returns `Result`/`Result<T>` → endpoint maps that to an
HTTP response. Handlers depend on `Domain` repository interfaces and `Application`-level abstractions
(e.g. `IUnitOfWork`, `IIdentityProviderService`), never directly on EF Core/Dapper types.

**Cross-module communication is outbox/inbox-based, not direct calls**, to keep modules decoupled at
runtime:
1. A domain entity raises an `IDomainEvent` (`Entity.Raise`). `InsertOutboxMessagesInterceptor` persists it
   as a row in that module's `outbox_messages` table (Postgres schema per module, e.g. `users`) at
   `SaveChanges` time, in the same transaction as the business data.
2. A Quartz `ProcessOutboxJob` (configured per-module, e.g.
   `Users.Infrastructure/Outbox/ProcessOutboxJob.cs`) polls unprocessed rows, resolves
   `IDomainEventHandler`s for that event type via reflection (`DomainEventHandlersFactory`), invokes them,
   and marks the row processed/errored. Handlers are wrapped in `IdempotentDomainEventHandler<T>` via
   Scrutor decoration (see `UsersModule.AddDomainEventHandlers`).
3. If a domain event handler needs to notify *other* modules, it publishes an `IIntegrationEvent` through
   `IEventBus` (MassTransit, in-memory bus). Consumers (`IIntegrationEventHandler`) are similarly
   discovered by reflection and decorated with `IdempotentIntegrationEventHandler<T>`, and inbound
   integration events land in that module's own inbox table (`ProcessInboxJob`) for idempotent processing.

**Persistence**: EF Core (Npgsql, snake_case naming convention via `EFCore.NamingConventions`) per module
for writes, each module in its own Postgres schema (`Schemas.cs`) with its own migrations history table.
Dapper (via `IDbConnectionFactory`) is used for the outbox/inbox polling jobs and other read-heavy/raw-SQL
paths rather than EF Core.

**Auth**: JWT bearer auth validated against Keycloak; permission-based authorization
(`IPermissionService`/`PermissionAuthorizationHandler`) where the `Users` module is the source of truth for
a user's permissions, fetched via `CustomClaimsTransformation` and cached (`ICacheService`, Redis-backed).

**Configuration**: each module contributes its own settings file(s) at the API layer — `modules.<name>.json`
+ `modules.<name>.Development.json` — loaded via `ConfigurationExtensions.AddModuleConfiguration`, keyed by
module name (`AddModuleConfiguration(["users", "events", "ticketing", "attendance"])` in `Program.cs`,
though only `users` files currently exist).
