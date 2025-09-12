namespace Fulbito.Core.Features.Authentication.Login;

public record LoginCommand( 
    string Email, 
    string Password
);

public record LoginResponse(
    bool Success,
    string Message,
    string? Token = null
);