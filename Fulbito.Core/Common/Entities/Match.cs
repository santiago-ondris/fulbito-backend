namespace Fulbito.Core.Common.Entities;

public class Match : BaseEntity
{
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public DateTime MatchDate { get; set; } = DateTime.UtcNow;
    
    // Relación con la liga
    public Guid LeagueId { get; set; }
    public League League { get; set; } = null!;
    
    // Navigation properties
    public ICollection<PlayerMatch> PlayerMatches { get; set; } = new List<PlayerMatch>();
}