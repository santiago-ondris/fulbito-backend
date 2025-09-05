namespace GestionLigas.Features.CrearLiga;

public class MetricaRequest
{
    public TipoMetrica Tipo { get; set; }
    public int Puntos { get; set; }
    public int? ParametroNumerico { get; set; } // Para rachas, diferencias de goles, etc.
}