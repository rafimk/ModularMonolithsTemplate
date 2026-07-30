using CompanyName.Common.Application.EventBus;
using CompanyName.Common.Application.Exceptions;
using CompanyName.Common.Application.Messaging;
using CompanyName.Common.Domain;
using CompanyName.Modules.Users.Application.Users.GetUser;
using CompanyName.Modules.Users.Domain.Users;
using CompanyName.Modules.Users.IntegrationEvents;
using MediatR;

namespace CompanyName.Modules.Users.Application.Users.RegisterUser;

internal sealed class UserRegisteredDomainEventHandler(ISender sender, IEventBus bus)
    : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override async Task Handle(
        UserRegisteredDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        Result<UserResponse> result = await sender.Send(
            new GetUserQuery(domainEvent.UserId),
            cancellationToken);

        if (result.IsFailure)
        {
            throw new EventlyException(nameof(GetUserQuery), result.Error);
        }

        await bus.PublishAsync(
            new UserRegisteredIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                result.Value.Id,
                result.Value.Email,
                result.Value.FirstName,
                result.Value.LastName),
            cancellationToken);
    }
}
