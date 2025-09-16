using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.ViewMatchups;

public static class ViewMatchupsEndpoint
{
    public static void MapViewMatchupsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/leagues/{leagueSlug}/matchups", async (
            string leagueSlug,
            Guid player1Id,
            Guid player2Id,
            ViewMatchupsHandler handler,
            ViewMatchupsValidator validator) =>
        {
            var query = new ViewMatchupsQuery
            {
                LeagueSlug = leagueSlug,
                Player1Id = player1Id,
                Player2Id = player2Id
            };

            // Validar
            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            // Ejecutar
            var result = await handler.Handle(query);
            
            return result.Success 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        });
    }
}