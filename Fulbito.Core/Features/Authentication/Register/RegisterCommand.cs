namespace Fulbito.Core.Features.Authentication.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
);

public record RegisterResponse(
    bool Success,
    string Message,
    Guid? UserId = null
);