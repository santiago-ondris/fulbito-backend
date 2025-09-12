namespace Fulbito.Core.Features.ManageLeague;

// ========== ADD PLAYER ==========
public record AddPlayerCommand
{
    public Guid LeagueId { get; set; }
    public Guid UserId { get; set; } // Setteado por el handler desde JWT
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public record AddPlayerResponse(
    bool Success,
    string Message,
    Guid? PlayerId = null
);

// ========== EDIT PLAYER ==========
public record EditPlayerCommand
{
    public Guid LeagueId { get; set; }
    public Guid PlayerId { get; set; }
    public Guid UserId { get; set; } // Setteado por el handler desde JWT
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public record EditPlayerResponse(
    bool Success,
    string Message
);

// ========== DELETE PLAYER ==========
public record DeletePlayerCommand
{
    public Guid LeagueId { get; set; }
    public Guid PlayerId { get; set; }
    public Guid UserId { get; set; } // Setteado por el handler desde JWT
}

public record DeletePlayerResponse(
    bool Success,
    string Message
);

// ========== REQUEST OBJECTS FOR ENDPOINTS ==========
public record AddPlayerRequest(
    string FirstName,
    string LastName
);

public record EditPlayerRequest(
    string FirstName,
    string LastName
);