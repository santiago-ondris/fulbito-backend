using Fulbito.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace Fulbito.Core.Features.ViewLeague;

public class ViewLeagueHandler
{
    private readonly ApplicationDbContext _context;

    public ViewLeagueHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ViewLeagueResponse?> Handle(ViewLeagueQuery query)
    {
        // Buscar liga con todos los datos relacionados
        var league = await _context.Leagues
            .Include(l => l.Players)
            .Include(l => l.Matches)
                .ThenInclude(m => m.PlayerMatches)
                    .ThenInclude(pm => pm.Player)
            .FirstOrDefaultAsync(l => l.Slug == query.Slug);

        if (league == null)
            return null;

        // Calcular tabla de posiciones
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

        return new ViewLeagueResponse
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
                PointsPerLossInStreak = league.PointsPerLossInStreak,
                IsMvpEnabled = league.IsMvpEnabled,
                PointsPerMvp = league.PointsPerMvp
            },
            PlayerStandings = playerStandings,
            Matches = matchSummaries
        };
    } 
}