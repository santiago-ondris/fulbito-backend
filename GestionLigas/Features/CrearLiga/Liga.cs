namespace GestionLigas.Features.CrearLiga;

public class Liga
{
    public int Id {get; set;}
    public string Nombre {get; set;} = string.Empty;
    public string UsuarioId {get; set;} = string.Empty;
    public DateTime FechaCreacion {get; set;}
    public ConfiguracionLiga Configuracion {get; set;} = null!;
}