using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionLigas.Infrastructure;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private ISender? _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}