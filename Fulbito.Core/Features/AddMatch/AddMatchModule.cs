using Microsoft.Extensions.DependencyInjection;

namespace Fulbito.Core.Features.AddMatch;

public static class AddMatchModule
{
    public static IServiceCollection AddMatchFeatures(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<AddMatchHandler>();

        // Validators
        services.AddScoped<AddMatchValidator>();
        
        return services;
    }
}