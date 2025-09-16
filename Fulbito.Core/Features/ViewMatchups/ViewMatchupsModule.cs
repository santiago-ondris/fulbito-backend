using Microsoft.Extensions.DependencyInjection;

namespace Fulbito.Core.Features.ViewMatchups;

public static class ViewMatchupsModule
{
    public static IServiceCollection AddViewMatchupsFeatures(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<ViewMatchupsHandler>();

        // Validators
        services.AddScoped<ViewMatchupsValidator>();
        
        return services;
    }
}