namespace Fulbito.Core.Common.Entities;

public class PlayerMatch : BaseEntity
{
    // Relaciones
    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    
    public Guid MatchId { get; set; }
    public Match Match { get; set; } = null!;
    
    // Datos del partido
    public int TeamNumber { get; set; } // 1 o 2
    public int Goals { get; set; } = 0; // Solo si está habilitado en la liga
    public bool IsMvp { get; set; } = false;
    
    // Resultado para este jugador (calculado)
    public MatchResult Result { get; set; }
}

public enum MatchResult
{
    Win,
    Draw,
    Loss
}