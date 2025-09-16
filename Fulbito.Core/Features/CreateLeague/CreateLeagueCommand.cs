namespace Fulbito.Core.Features.CreateLeague;

public record CreateLeagueCommand
{
    // Datos básicos
    public string Name { get; set; } = string.Empty;
    public int PlayersPerTeam { get; set; }
    
    // Puntajes obligatorios
    public int PointsPerWin { get; set; }
    public int PointsPerDraw { get; set; }
    public int PointsPerLoss { get; set; }
    public int PointsPerMatchPlayed { get; set; }
    
    // Métricas opcionales
    public bool IsGoalsEnabled { get; set; }
    public int PointsPerGoal { get; set; } // Solo si IsGoalsEnabled = true
    public bool IsMvpEnabled { get; set; }
    public int PointsPerMvp { get; set; }
    
    public bool IsWinStreakEnabled { get; set; }
    public int PointsPerWinInStreak { get; set; } // Solo si IsWinStreakEnabled = true
    public int MinWinStreakToActivate { get; set; }
    
    public bool IsLossStreakEnabled { get; set; }
    public int PointsPerLossInStreak { get; set; } // Solo si IsLossStreakEnabled = true
    public int MinLossStreakToActivate { get; set; }
    
    // Jugadores iniciales
    public List<CreatePlayerRequest> Players { get; set; } = new();
}

public record CreatePlayerRequest(string FirstName, string LastName);

public record CreateLeagueResponse(
    bool Success,
    string Message,
    Guid? LeagueId = null,
    string? Slug = null
);