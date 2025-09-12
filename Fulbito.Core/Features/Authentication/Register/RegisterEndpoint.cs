using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.Authentication.Register;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
            [FromBody] RegisterCommand command,
            RegisterHandler handler,
            RegisterValidator validator) =>
        {
            // Validar
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            // Ejecutar
            var result = await handler.Handle(command);
            
            return result.Success 
                ? Results.Ok(result) 
                : Results.BadRequest(result);
        });
    }
}