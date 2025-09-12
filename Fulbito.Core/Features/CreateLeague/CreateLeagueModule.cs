using Microsoft.Extensions.DependencyInjection;

namespace Fulbito.Core.Features.CreateLeague;

public static class CreateLeagueModule
{
    public static IServiceCollection AddCreateLeagueFeatures(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<CreateLeagueHandler>();
        
        // Validators
        services.AddScoped<CreateLeagueValidator>();
        
        return services;
    }
}