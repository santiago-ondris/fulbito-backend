using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.ViewLeague;

public static class ViewLeagueEndpoint
{
    public static void MapViewLeagueEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/leagues/{slug}", async (
            string slug,
            ViewLeagueHandler handler) =>
        {
            var query = new ViewLeagueQuery(slug);
            var result = await handler.Handle(query);
            
            return result != null 
                ? Results.Ok(result)
                : Results.NotFound(new { message = "Liga no encontrada" });
        }); // Sin .RequireAuthorization() porque es público
    }
}