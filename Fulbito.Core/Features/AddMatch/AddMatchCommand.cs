namespace Fulbito.Core.Features.AddMatch;

public record AddMatchCommand
{
    public Guid LeagueId { get; set; }
    public Guid UserId { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public DateTime MatchDate { get; set; } = DateTime.UtcNow;
    
    public List<PlayerInTeamRequest> Team1Players { get; set; } = new();
    public List<PlayerInTeamRequest> Team2Players { get; set; } = new();
    public string? MvpPlayerId { get; set; }
}

public record PlayerInTeamRequest
{
    // Para jugador existente
    public Guid? PlayerId { get; set; }
    
    // Para jugador nuevo
    public NewPlayerRequest? NewPlayer { get; set; }
    
    // Goles del jugador (solo si la liga tiene goles habilitados)
    public int Goals { get; set; } = 0;
}

public record NewPlayerRequest(
    string FirstName,
    string LastName
);

public record AddMatchResponse(
    bool Success,
    string Message,
    Guid? MatchId = null
);