namespace Fulbito.Core.Features.AdminLeague.GetMyLeagues;

public record GetMyLeaguesQuery();

public record GetMyLeaguesResponse
{
    public List<MyLeagueSummary> Leagues { get; set; } = new();
}

public record MyLeagueSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int PlayersPerTeam { get; set; }
    public int TotalPlayers { get; set; }
    public int TotalMatches { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastMatchDate { get; set; }
}