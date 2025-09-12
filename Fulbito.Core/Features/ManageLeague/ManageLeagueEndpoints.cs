using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.ManageLeague;

public static class ManageLeagueEndpoints
{
    public static void MapManageLeagueEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAddPlayerEndpoint();
        app.MapEditPlayerEndpoint();
        app.MapDeletePlayerEndpoint();
    }

    // ========== ADD PLAYER ENDPOINT ==========
    public static void MapAddPlayerEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/leagues/{leagueId:guid}/players", async (
            Guid leagueId,
            [FromBody] AddPlayerRequest request,
            ManageLeagueHandler handler,
            AddPlayerValidator validator) =>
        {
            var command = new AddPlayerCommand
            {
                LeagueId = leagueId,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            // Validar
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            // Ejecutar
            var result = await handler.Handle(command);
            
            return result.Success 
                ? Results.Created($"/api/leagues/{leagueId}/players/{result.PlayerId}", result)
                : Results.BadRequest(result);
        })
        .RequireAuthorization();
    }

    // ========== EDIT PLAYER ENDPOINT ==========
    public static void MapEditPlayerEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/leagues/{leagueId:guid}/players/{playerId:guid}", async (
            Guid leagueId,
            Guid playerId,
            [FromBody] EditPlayerRequest request,
            ManageLeagueHandler handler,
            EditPlayerValidator validator) =>
        {
            var command = new EditPlayerCommand
            {
                LeagueId = leagueId,
                PlayerId = playerId,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            // Validar
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            // Ejecutar
            var result = await handler.Handle(command);
            
            return result.Success 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .RequireAuthorization();
    }

    // ========== DELETE PLAYER ENDPOINT ==========
    public static void MapDeletePlayerEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/leagues/{leagueId:guid}/players/{playerId:guid}", async (
            Guid leagueId,
            Guid playerId,
            ManageLeagueHandler handler,
            DeletePlayerValidator validator) =>
        {
            var command = new DeletePlayerCommand
            {
                LeagueId = leagueId,
                PlayerId = playerId
            };

            // Validar
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            // Ejecutar
            var result = await handler.Handle(command);
            
            return result.Success 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .RequireAuthorization();
    }
}