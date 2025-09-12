using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.ViewLeague;

public static class ViewLeagueEndpoints
{
    public static void MapViewLeagueEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapViewLeagueEndpoint();
    }
}