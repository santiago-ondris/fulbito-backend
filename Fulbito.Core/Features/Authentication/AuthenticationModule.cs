using Microsoft.Extensions.DependencyInjection;
using Fulbito.Core.Features.Authentication.Register;
using Fulbito.Core.authentication.Login;

namespace Fulbito.Core.Features.Authentication;

public static class AuthenticationModule
{
    public static IServiceCollection AddAuthenticationFeatures(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<RegisterHandler>();
        services.AddScoped<Login.LoginHandler>();
        
        // Validators
        services.AddScoped<RegisterValidator>();
        services.AddScoped<LoginValidator>();
        
        return services;
    }
}