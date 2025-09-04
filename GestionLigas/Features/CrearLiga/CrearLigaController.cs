using GestionLigas.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace GestionLigas.Features.CrearLiga;

[Route("api/[Controller]")]
public class LigasController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CrearLiga([FromBody] CrearLigaRequest request)
    {
        var response = await Mediator.Send(request);
        return Ok(response);
    }
}