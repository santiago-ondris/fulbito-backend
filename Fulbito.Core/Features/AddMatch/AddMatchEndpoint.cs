using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.AddMatch;

public static class AddMatchEndpoint
{
    public static void MapAddMatchEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/leagues/{leagueId:guid}/matches", async (
            Guid leagueId,
            [FromBody] AddMatchRequest request,
            AddMatchHandler handler,
            AddMatchValidator validator) =>
        {
            // Crear el command con el leagueId del route
            var command = new AddMatchCommand
            {
                LeagueId = leagueId,
                Team1Score = request.Team1Score,
                Team2Score = request.Team2Score,
                MatchDate = request.MatchDate,
                Team1Players = request.Team1Players,
                Team2Players = request.Team2Players
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
                ? Results.Created($"/api/leagues/{leagueId}/matches/{result.MatchId}", result)
                : Results.BadRequest(result);
        })
        .RequireAuthorization(); // Requiere JWT token
    }
}

// Request separado para el endpoint (sin UserId que es interno)
public record AddMatchRequest
{
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public DateTime MatchDate { get; set; } = DateTime.UtcNow;
    
    public List<PlayerInTeamRequest> Team1Players { get; set; } = new();
    public List<PlayerInTeamRequest> Team2Players { get; set; } = new();
}