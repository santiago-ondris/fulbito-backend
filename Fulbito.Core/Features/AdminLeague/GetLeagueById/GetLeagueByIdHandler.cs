using System.Security.Claims;
using Fulbito.Core.Database;
using Fulbito.Core.Features.ViewLeague;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fulbito.Core.Features.AdminLeague.GetLeagueById;

public class GetLeagueByIdHandler
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetLeagueByIdHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GetLeagueByIdResponse?> Handle(GetLeagueByIdQuery query)
    {
        // Obtener UserId del JWT
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return null; // Usuario no autenticado
        }

        // Buscar liga con todos los datos relacionados
        // IMPORTANTE: Validar que la liga pertenece al usuario autenticado
        var league = await _context.Leagues
            .Include(l => l.Players)
            .Include(l => l.Matches)
                .ThenInclude(m => m.PlayerMatches)
                    .ThenInclude(pm => pm.Player)
            .FirstOrDefaultAsync(l => l.Id == query.LeagueId && l.UserId == userId);

        if (league == null)
            return null;

        // Calcular tabla de posiciones usando el calculador existente
        var playerStandings = LeagueStatisticsCalcualtor.CalculatePlayerStandings(league);

        // Preparar historial de partidos
        var matchSummaries = league.Matches
            .OrderByDescending(m => m.MatchDate)
            .Select(match => new MatchSummary
            {
                MatchId = match.Id,
                MatchDate = match.MatchDate,
                Team1Score = match.Team1Score,
                Team2Score = match.Team2Score,
                Team1Players = match.PlayerMatches
                    .Where(pm => pm.TeamNumber == 1)
                    .Select(pm => new PlayerInMatch
                    {
                        PlayerId = pm.PlayerId,
                        FirstName = pm.Player.FirstName,
                        LastName = pm.Player.LastName,
                        Goals = league.IsGoalsEnabled ? pm.Goals : 0
                    }).ToList(),
                Team2Players = match.PlayerMatches
                    .Where(pm => pm.TeamNumber == 2)
                    .Select(pm => new PlayerInMatch
                    {
                        PlayerId = pm.PlayerId,
                        FirstName = pm.Player.FirstName,
                        LastName = pm.Player.LastName,
                        Goals = league.IsGoalsEnabled ? pm.Goals : 0
                    }).ToList()
            }).ToList();

        // Crear response reutilizando la estructura existente
        return new GetLeagueByIdResponse
        {
            Id = league.Id,
            Name = league.Name,
            Slug = league.Slug,
            PlayersPerTeam = league.PlayersPerTeam,
            Scoring = new LeagueScoring
            {
                PointsPerWin = league.PointsPerWin,
                PointsPerDraw = league.PointsPerDraw,
                PointsPerLoss = league.PointsPerLoss,
                PointsPerMatchPlayed = league.PointsPerMatchPlayed,
                IsGoalsEnabled = league.IsGoalsEnabled,
                PointsPerGoal = league.PointsPerGoal,
                IsWinStreakEnabled = league.IsWinStreakEnabled,
                PointsPerWinInStreak = league.PointsPerWinInStreak,
                IsLossStreakEnabled = league.IsLossStreakEnabled,
                PointsPerLossInStreak = league.PointsPerLossInStreak
            },
            PlayerStandings = playerStandings,
            Matches = matchSummaries
        };
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }
}