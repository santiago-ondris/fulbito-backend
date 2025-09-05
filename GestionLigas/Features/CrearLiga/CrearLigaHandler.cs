using GestionLigas.Infrastructure;
using MediatR;

namespace GestionLigas.Features.CrearLiga;

public class CrearLigaHandler : IRequestHandler<CrearLigaRequest, CrearLigaResponse>
{
    private readonly ApplicationDbContext _context;
    public CrearLigaHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CrearLigaResponse> Handle(CrearLigaRequest request, CancellationToken cancellationToken)
    {
        // Por ahora usuario hardcodeado
        var usuarioId = "usuario-temporal";

        var liga = new Liga
        {
            Nombre = request.Nombre,
            UsuarioId = usuarioId,
            FechaCreacion = DateTime.UtcNow,
            Configuracion = new ConfiguracionLiga
            {
                Formato = request.Formato,
                ArqueroObligatorio = request.ArqueroObligatorio,
                CantidadMinimaArqueros = request.CantidadMinimaArqueros,
                DiasJuego = request.DiasJuego,
                Horarios = request.Horarios,
                ReglasAdicionales = request.ReglasAdicionales
            }
        };

        foreach (var metricaRequest in request.Metricas)
        {
            var metrica = new MetricaLiga
            {
                Liga = liga,
                Tipo = metricaRequest.Tipo,
                Puntos = metricaRequest.Puntos,
                ParametroNumerico = metricaRequest.ParametroNumerico,
                EsObligatoria = EsMetricaObligatoria(metricaRequest.Tipo)
            };
            _context.MetricasLiga.Add(metrica);
        }

        _context.Ligas.Add(liga);
        await _context.SaveChangesAsync(cancellationToken);

        return new CrearLigaResponse
        {
            LigaId = liga.Id,
            Mensaje = $"Liga '{liga.Nombre}' creada exitosamente"
        };
    }
    
    private static bool EsMetricaObligatoria(TipoMetrica tipo)
    {
        return tipo is TipoMetrica.PartidoJugado or 
                    TipoMetrica.PartidoGanado or 
                    TipoMetrica.PartidoEmpatado or 
                    TipoMetrica.PartidoPerdido;
    }
}