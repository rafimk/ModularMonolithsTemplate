using CompanyName.Common.Application.Messaging;

namespace CompanyName.Modules.Users.Application.Users.GetUser;

public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;
