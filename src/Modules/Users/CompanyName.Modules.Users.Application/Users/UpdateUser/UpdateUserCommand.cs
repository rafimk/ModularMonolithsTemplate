using CompanyName.Common.Application.Messaging;

namespace CompanyName.Modules.Users.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(Guid UserId, string FirstName, string LastName) : ICommand;
