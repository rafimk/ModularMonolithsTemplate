# ModularMonolithsTemplate

A .NET 10 modular monolith template — CQRS/MediatR, per-module EF Core + Dapper, outbox/inbox integration
events, Keycloak auth, and a Docker Compose local dev stack. See `CLAUDE.md` for the full architecture
rundown.

## Using this as a template

```bash
dotnet new install .
dotnet new modular-monolith -n YourCompanyName -o path/to/output
```

This renames every `CompanyName.*` occurrence (namespaces, files, folders, Docker/Keycloak config) to your
name, regenerates the API's `UserSecretsId`, and lowercases the Docker image tag.

## Local dev stack

```bash
docker compose up
```

Starts the API, Postgres, Keycloak (imports the `CompanyName` realm on first boot), Seq, Redis, and Jaeger.
The HTTPS dev cert needs a one-time export before the API container can bind its HTTPS port — see
`CLAUDE.md` for the exact command.

## Scaffolding a new module

Eight class libraries per module, following the `Users` module as the reference implementation:

```
dotnet new classlib   -n CompanyName.Modules.Users.Domain -o src/Modules/Users/CompanyName.Modules.Users.Domain   -f net10.0
dotnet new classlib   -n CompanyName.Modules.Users.Application -o src/Modules/Users/CompanyName.Modules.Users.Application -f net10.0
dotnet new classlib   -n CompanyName.Modules.Users.ArchitectureTests -o src/Modules/Users/CompanyName.Modules.Users.ArchitectureTests -f net10.0
dotnet new classlib   -n CompanyName.Modules.Users.Infrastructure -o src/Modules/Users/CompanyName.Modules.Users.Infrastructure -f net10.0
dotnet new classlib   -n CompanyName.Modules.Users.IntegrationEvents -o src/Modules/Users/CompanyName.Modules.Users.IntegrationEvents -f net10.0
dotnet new classlib   -n CompanyName.Modules.Users.IntegrationTests -o src/Modules/Users/CompanyName.Modules.Users.IntegrationTests -f net10.0
dotnet new classlib   -n CompanyName.Modules.Users.Presentation -o src/Modules/Users/CompanyName.Modules.Users.Presentation -f net10.0
dotnet new classlib   -n CompanyName.Modules.Users.UnitTests -o src/Modules/Users/CompanyName.Modules.Users.UnitTests -f net10.0
```
