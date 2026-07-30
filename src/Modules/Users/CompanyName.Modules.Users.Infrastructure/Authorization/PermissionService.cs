using CompanyName.Common.Application.Authorization;
using CompanyName.Common.Domain;
using CompanyName.Modules.Users.Application.Users.GetUserPermissions;
using MediatR;

namespace CompanyName.Modules.Users.Infrastructure.Authorization;

internal sealed class PermissionService(ISender sender) : IPermissionService
{
    public async Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId)
    {
        return await sender.Send(new GetUserPermissionsQuery(identityId));
    }
}
