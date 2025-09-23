namespace Fulbito.Core.Features.ViewMatchups;

public record ViewMatchupsQuery
{
    public string LeagueSlug { get; set; } = string.Empty;
    public Guid Player1Id { get; set; }
    public Guid Player2Id { get; set; }
}

public record ViewMatchupsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public MatchupData? Data { get; set; }
}

public record MatchupData
{
    public PlayerSummary Player1 { get; set; } = new();
    public PlayerSummary Player2 { get; set; } = new();
    public MatchupStats Stats { get; set; } = new();
    public List<MatchupHistory> Matches { get; set; } = new();
}

public record PlayerSummary
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public record MatchupStats
{
    public int Player1Wins { get; set; }
    public int Player2Wins { get; set; }
    public int Draws { get; set; }
    public int TotalMatches { get; set; }
    public string Summary { get; set; } = string.Empty;
}

public record MatchupHistory
{
    public Guid MatchId { get; set; }
    public DateTime MatchDate { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public string Result { get; set; } = string.Empty;
    public PlayerMatchDetails Player1Details { get; set; } = new();
    public PlayerMatchDetails Player2Details { get; set; } = new();
}

public record PlayerMatchDetails
{
    public int Goals { get; set; }
    public bool WasInWinningTeam { get; set; }
}