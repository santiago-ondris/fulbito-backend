using Microsoft.AspNetCore.Identity;

namespace Fulbito.Core.Common.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<League> Leagues { get; set; } = new List<League>();
}