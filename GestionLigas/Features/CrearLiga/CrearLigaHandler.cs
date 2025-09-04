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
                PosicionesDisponibles = request.PosicionesDisponibles,
                DiasJuego = request.DiasJuego,
                Horarios = request.Horarios,
                ReglasAdicionales = request.ReglasAdicionales
            }
        };

        _context.Ligas.Add(liga);
        await _context.SaveChangesAsync(cancellationToken);

        return new CrearLigaResponse 
        {
            LigaId = liga.Id,
            Mensaje = $"Liga '{liga.Nombre}' creada exitosamente"
        };
    }
}