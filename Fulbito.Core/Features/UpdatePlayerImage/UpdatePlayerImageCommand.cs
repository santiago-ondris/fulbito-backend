using Microsoft.AspNetCore.Http;

namespace Fulbito.Core.Features.UpdatePlayerImage;

public record UpdatePlayerImageCommand(
    Guid LeagueId,
    Guid PlayerId,
    IFormFile ImageFile
);

public record UpdatePlayerImageResponse(
    bool Success,
    string Message,
    string? ImageUrl = null
);