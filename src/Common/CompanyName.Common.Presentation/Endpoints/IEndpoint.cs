using Microsoft.AspNetCore.Routing;

namespace CompanyName.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
