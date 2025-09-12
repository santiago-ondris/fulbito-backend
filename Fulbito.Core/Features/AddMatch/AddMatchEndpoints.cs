using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.AddMatch;

public static class AddMatchEndpoints
{
    public static void MapAddMatchEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAddMatchEndpoint();
    }
}