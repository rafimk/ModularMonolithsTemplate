using CompanyName.Common.Domain;
using CompanyName.Common.Presentation.Endpoints;
using CompanyName.Common.Presentation.Results;
using CompanyName.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CompanyName.Modules.Users.Presentation.Users;

internal sealed class RegisterUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (Request request, ISender sender) =>
        {
            Result<Guid> result = await sender.Send(new RegisterUserCommand(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Users);
    }

    internal sealed class Request
    {
        public required string Email { get; init; }

        public required string Password { get; init; }

        public required string FirstName { get; init; }

        public required string LastName { get; init; }
    }
}
