using CompanyName.Common.Application.Authorization;
using CompanyName.Common.Application.Messaging;

namespace CompanyName.Modules.Users.Application.Users.GetUserPermissions;

public sealed record GetUserPermissionsQuery(string IdentityId) : IQuery<PermissionsResponse>;
