using Microsoft.Extensions.DependencyInjection;

namespace Fulbito.Core.Features.ManageLeague;

public static class ManageLeagueModule
{
    public static IServiceCollection AddManageLeagueFeatures(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<ManageLeagueHandler>();
        
        // Validators
        services.AddScoped<AddPlayerValidator>();
        services.AddScoped<EditPlayerValidator>();
        services.AddScoped<DeletePlayerValidator>();
        
        return services;
    }
}