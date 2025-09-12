using Fulbito.Core.Common.Entities;
using Fulbito.Core.Features.ViewLeague;

public class LeagueStatisticsCalcualtor
{
    public static List<PlayerStanding> CalculatePlayerStandings(League league)
    {
        var totalMatches = league.Matches.Count;
        
        var standings = league.Players.Select(player =>
        {
            var playerMatches = player.PlayerMatches.ToList();
            
            // Métricas básicas
            var matchesPlayed = playerMatches.Count;
            var matchesWon = playerMatches.Count(pm => pm.Result == MatchResult.Win);
            var matchesDrawn = playerMatches.Count(pm => pm.Result == MatchResult.Draw);
            var matchesLost = playerMatches.Count(pm => pm.Result == MatchResult.Loss);
            var goalsFor = league.IsGoalsEnabled ? playerMatches.Sum(pm => pm.Goals) : (int?)null;
            
            // Calcular rachas actuales
            var currentWinStreak = league.IsWinStreakEnabled ? CalculateCurrentWinStreak(playerMatches) : (int?)null;
            var currentLossStreak = league.IsLossStreakEnabled ? CalculateCurrentLossStreak(playerMatches) : (int?)null;
            
            // Calcular puntaje total
            var totalPoints = CalculateTotalPoints(league, matchesPlayed, matchesWon, matchesDrawn, matchesLost, 
                                                 goalsFor ?? 0, currentWinStreak ?? 0, currentLossStreak ?? 0);
            
            // Calcular tasas
            var attendanceRate = totalMatches > 0 ? (decimal)matchesPlayed / totalMatches * 100 : 0;
            var winRate = matchesPlayed > 0 ? (decimal)matchesWon / matchesPlayed * 100 : 0;
            var drawRate = matchesPlayed > 0 ? (decimal)matchesDrawn / matchesPlayed * 100 : 0;
            var lossRate = matchesPlayed > 0 ? (decimal)matchesLost / matchesPlayed * 100 : 0;

            return new PlayerStanding
            {
                PlayerId = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                TotalPoints = totalPoints,
                MatchesPlayed = matchesPlayed,
                MatchesWon = matchesWon,
                MatchesDrawn = matchesDrawn,
                MatchesLost = matchesLost,
                GoalsFor = goalsFor,
                CurrentWinStreak = currentWinStreak,
                CurrentLossStreak = currentLossStreak,
                AttendanceRate = Math.Round(attendanceRate, 1),
                WinRate = Math.Round(winRate, 1),
                DrawRate = Math.Round(drawRate, 1),
                LossRate = Math.Round(lossRate, 1)
            };
        }).ToList();

        // Aplicar criterios de desempate
        return standings
            .OrderByDescending(p => p.TotalPoints)
            .ThenByDescending(p => p.MatchesWon)
            .ThenByDescending(p => p.GoalsFor ?? 0)
            .ThenBy(p => p.FirstName)
            .ToList();
    }

    public static int CalculateTotalPoints(League league, int matchesPlayed, int matchesWon, int matchesDrawn, 
                                   int matchesLost, int goalsFor, int currentWinStreak, int currentLossStreak)
    {
        var total = 0;
        
        // Puntajes obligatorios
        total += matchesPlayed * league.PointsPerMatchPlayed;
        total += matchesWon * league.PointsPerWin;
        total += matchesDrawn * league.PointsPerDraw;
        total += matchesLost * league.PointsPerLoss;
        
        // Puntajes opcionales
        if (league.IsGoalsEnabled)
            total += goalsFor * league.PointsPerGoal;
            
        if (league.IsWinStreakEnabled && currentWinStreak > 0)
            total += currentWinStreak * league.PointsPerWinInStreak;
            
        if (league.IsLossStreakEnabled && currentLossStreak > 0)
            total += currentLossStreak * league.PointsPerLossInStreak; // Esto debería ser negativo
            
        return total;
    }

    public static int CalculateCurrentWinStreak(List<PlayerMatch> playerMatches)
    {
        var orderedMatches = playerMatches.OrderByDescending(pm => pm.Match.MatchDate).ToList();
        var streak = 0;
        
        foreach (var match in orderedMatches)
        {
            if (match.Result == MatchResult.Win)
                streak++;
            else
                break; // Se corta la racha
        }
        
        return streak;
    }

    public static int CalculateCurrentLossStreak(List<PlayerMatch> playerMatches)
    {
        var orderedMatches = playerMatches.OrderByDescending(pm => pm.Match.MatchDate).ToList();
        var streak = 0;
        
        foreach (var match in orderedMatches)
        {
            if (match.Result == MatchResult.Loss)
                streak++;
            else
                break; // Se corta la racha
        }
        
        return streak;
    }
}