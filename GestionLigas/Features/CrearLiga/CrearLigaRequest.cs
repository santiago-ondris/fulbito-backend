using MediatR;

namespace GestionLigas.Features.CrearLiga;

public class CrearLigaRequest : IRequest<CrearLigaResponse>
{
    public string Nombre {get; set;} = string.Empty;
    public string Formato {get; set;} = string.Empty; // 8vs8, 5vs5, etc.
    public bool ArqueroObligatorio {get; set;}
    public int CantidadMinimaArqueros {get; set;}
    public List<MetricaRequest> Metricas { get; set; } = new();
    public List<string> DiasJuego { get; set; } = new(); 
    public string? Horarios {get; set;}
    public string? ReglasAdicionales {get; set;}
}