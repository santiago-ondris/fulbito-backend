using Microsoft.Extensions.DependencyInjection;

namespace Fulbito.Core.Features.ViewLeague;

public static class ViewLeagueModule
{
    public static IServiceCollection AddViewLeagueFeatures(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<ViewLeagueHandler>();
        
        return services;
    }
}