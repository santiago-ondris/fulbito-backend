using Microsoft.Extensions.DependencyInjection;
using Fulbito.Core.Features.AdminLeague.GetLeagueById;
using Fulbito.Core.Features.AdminLeague.GetMyLeagues;

namespace Fulbito.Core.Features.AdminLeague;

public static class AdminLeagueModule
{
    public static IServiceCollection AddAdminLeagueFeatures(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<GetLeagueByIdHandler>();
        services.AddScoped<GetMyLeaguesHandler>();
        
        return services;
    }
}