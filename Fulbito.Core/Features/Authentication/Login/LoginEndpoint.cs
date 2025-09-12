using Fulbito.Core.authentication.Login;
using Fulbito.Core.Features.Authentication.Login;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            [FromBody] LoginCommand command,
            LoginHandler handler,
            LoginValidator validator) =>
        {
            var validationResult = await validator.ValidateAsync(command);
            if(!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var result = await handler.Handle(command);

            return result.Success
                ? Results.Ok(result)
                : Results.BadRequest(result);
        });
    }
}