using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.CreateLeague;

public static class CreateLeagueEndpoint
{
    public static void MapCreateLeagueEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/leagues", async (
            [FromBody] CreateLeagueCommand command,
            CreateLeagueHandler handler,
            CreateLeagueValidator validator) =>
        {
            // Validar
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            // Ejecutar
            var result = await handler.Handle(command);
            
            return result.Success 
                ? Results.Created($"/api/leagues/{result.LeagueId}", result)
                : Results.BadRequest(result);
        })
        .RequireAuthorization(); // Requiere JWT token
    }
}