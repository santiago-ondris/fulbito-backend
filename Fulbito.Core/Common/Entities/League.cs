namespace Fulbito.Core.Common.Entities;

public class League : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int PlayersPerTeam { get; set; }
    
    // Administrador de la liga
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    // Configuración de métricas obligatorias (puntajes)
    public int PointsPerWin { get; set; }
    public int PointsPerDraw { get; set; }
    public int PointsPerLoss { get; set; }
    public int PointsPerMatchPlayed { get; set; }
    
    // Configuración de métricas opcionales
    public bool IsGoalsEnabled { get; set; }
    public int PointsPerGoal { get; set; }

    public bool IsMvpEnabled { get; set; }
    public int PointsPerMvp { get; set; }
    
    public bool IsWinStreakEnabled { get; set; }
    public int PointsPerWinInStreak { get; set; }
    
    public bool IsLossStreakEnabled { get; set; }
    public int PointsPerLossInStreak { get; set; } // Valor negativo
    
    // Navigation properties
    public ICollection<Player> Players { get; set; } = new List<Player>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}