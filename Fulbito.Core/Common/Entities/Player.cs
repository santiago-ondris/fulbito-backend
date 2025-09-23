namespace Fulbito.Core.Common.Entities;

public class Player : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // Relación con la liga
    public Guid LeagueId { get; set; }
    public League League { get; set; } = null!;
    
    public string? ImageUrl { get; set; }
    
    // Navigation properties
    public ICollection<PlayerMatch> PlayerMatches { get; set; } = new List<PlayerMatch>();
}