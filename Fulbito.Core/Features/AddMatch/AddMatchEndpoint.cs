using System.Security.Claims;
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
            AddMatchValidator validator,
            IHttpContextAccessor httpContextAccessor) => 
        {
            Console.WriteLine($"=== ENDPOINT DEBUG ===");
            Console.WriteLine($"LeagueId: {leagueId}");
            Console.WriteLine($"Request received: {request != null}");
            
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }
            
            var command = new AddMatchCommand
            {
                LeagueId = leagueId,
                UserId = userId,
                Team1Score = request.Team1Score,
                Team2Score = request.Team2Score,
                MatchDate = request.MatchDate,
                Team1Players = request.Team1Players,
                Team2Players = request.Team2Players
            };

            Console.WriteLine($"Command created with UserId: {userId}, validating...");
            
            // Validar (ahora debería pasar)
            var validationResult = await validator.ValidateAsync(command);
            Console.WriteLine($"Validation passed: {validationResult.IsValid}");
            
            if(!validationResult.IsValid)
            {
                Console.WriteLine($"Validation errors: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
                return Results.BadRequest(validationResult.Errors);
            }

            Console.WriteLine($"Calling handler...");
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