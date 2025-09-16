using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.ViewMatchups;

public static class ViewMatchupsEndpoints
{
    public static void MapViewMatchupsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapViewMatchupsEndpoint();
    }
}