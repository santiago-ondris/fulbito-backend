using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.CreateLeague;

public static class CreateLeagueEndpoints
{
    public static void MapCreateLeagueEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateLeagueEndpoint();
    }
}