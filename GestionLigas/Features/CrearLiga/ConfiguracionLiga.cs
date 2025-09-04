namespace GestionLigas.Features.CrearLiga;

public class ConfiguracionLiga
{
    public int Id { get; set; }
    public int LigaId { get; set; }
    
    public string Formato { get; set; } = string.Empty;
    public bool ArqueroObligatorio { get; set; }
    public int CantidadMinimaArqueros { get; set; }
    public List<string> PosicionesDisponibles { get; set; } = new();
    public List<string> DiasJuego { get; set; } = new();
    public string? Horarios { get; set; }
    public string? ReglasAdicionales { get; set; }
    
    // Navigation
    public Liga Liga { get; set; } = null!;
}