using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fulbito.Core.Features.UpdatePlayerImage;

[ApiController]
[Route("api/leagues/{leagueId}/players/{playerId}")]
[Authorize]
public class UpdatePlayerImageController : ControllerBase
{
    private readonly UpdatePlayerImageHandler _handler;
    private readonly UpdatePlayerImageValidator _validator;

    public UpdatePlayerImageController(
        UpdatePlayerImageHandler handler,
        UpdatePlayerImageValidator validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("image")]
    public async Task<IActionResult> UpdatePlayerImage(
        [FromRoute] Guid leagueId,
        [FromRoute] Guid playerId,
        [FromForm] IFormFile image)
    {
        var command = new UpdatePlayerImageCommand(leagueId, playerId, image);

        var validationResult = _validator.Validate(command);
        if (!validationResult.IsValid)
        {
            return BadRequest(new UpdatePlayerImageResponse(false, string.Join(", ", validationResult.Errors)));
        }

        var result = await _handler.Handle(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}