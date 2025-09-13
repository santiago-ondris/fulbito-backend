using Microsoft.Extensions.DependencyInjection;
using Fulbito.Core.Features.AdminLeague.GetLeagueById;

namespace Fulbito.Core.Features.AdminLeague;

public static class AdminLeagueModule
{
    public static IServiceCollection AddAdminLeagueFeatures(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<GetLeagueByIdHandler>();
        
        return services;
    }
}