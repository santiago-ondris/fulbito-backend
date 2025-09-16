namespace Fulbito.Core.Features.ViewLeague;

// Solo el slug como input
public record ViewLeagueQuery(string Slug);

public record ViewLeagueResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int PlayersPerTeam { get; set; }
    
    // Configuración de puntajes (para mostrar cómo se calcula)
    public LeagueScoring Scoring { get; set; } = new();
    
    // Tabla de posiciones ordenada por puntaje total
    public List<PlayerStanding> PlayerStandings { get; set; } = new();
    
    // Historial de partidos (más recientes primero)
    public List<MatchSummary> Matches { get; set; } = new();
}

public record LeagueScoring
{
    // Puntajes obligatorios
    public int PointsPerWin { get; set; }
    public int PointsPerDraw { get; set; }
    public int PointsPerLoss { get; set; }
    public int PointsPerMatchPlayed { get; set; }
    
    // Métricas opcionales habilitadas
    public bool IsGoalsEnabled { get; set; }
    public int PointsPerGoal { get; set; }
    
    public bool IsWinStreakEnabled { get; set; }
    public int PointsPerWinInStreak { get; set; }
    public int MinWinStreakToActivate { get; set; }
    
    public bool IsLossStreakEnabled { get; set; }
    public int PointsPerLossInStreak { get; set; }
    public int MinLossStreakToActivate { get; set; }

    public bool IsMvpEnabled { get; set; }
    public int PointsPerMvp { get; set; }
}

public record PlayerStanding
{
    public Guid PlayerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    
    // Puntaje total calculado
    public int TotalPoints { get; set; }
    
    // Métricas obligatorias
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesDrawn { get; set; }
    public int MatchesLost { get; set; }
    
    // Métricas opcionales (solo si están habilitadas)
    public int? GoalsFor { get; set; }
    public int? CurrentWinStreak { get; set; }
    public int? CurrentLossStreak { get; set; }
    
    // Tasas calculadas (solo estadísticas, no puntaje)
    public decimal AttendanceRate { get; set; } // (partidos jugados / total partidos liga) * 100
    public decimal WinRate { get; set; } // (ganados / jugados) * 100
    public decimal DrawRate { get; set; } // (empatados / jugados) * 100
    public decimal LossRate { get; set; } // (perdidos / jugados) * 100
}

public record MatchSummary
{
    public Guid MatchId { get; set; }
    public DateTime MatchDate { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    
    // Jugadores de cada equipo
    public List<PlayerInMatch> Team1Players { get; set; } = new();
    public List<PlayerInMatch> Team2Players { get; set; } = new();
}

public record PlayerInMatch
{
    public Guid PlayerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public int Goals { get; set; } // Solo si IsGoalsEnabled
}