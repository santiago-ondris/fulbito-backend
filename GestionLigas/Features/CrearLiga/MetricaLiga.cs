namespace GestionLigas.Features.CrearLiga;

public class MetricaLiga
{
    public int Id { get; set; }
    public int LigaId { get; set; }
    public TipoMetrica Tipo { get; set; }
    public int Puntos { get; set; }
    public bool EsObligatoria { get; set; } // true para PartidoJugado, PartidoGanado, etc.
    
    // Para métricas que requieren parámetros adicionales (ej: diferencia de goles, cantidad de partidos en racha)
    public int? ParametroNumerico { get; set; }
    
    // Navigation property
    public Liga Liga { get; set; } = null!;
}