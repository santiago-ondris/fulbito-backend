using Microsoft.Extensions.DependencyInjection;

namespace Fulbito.Core.Features.UpdatePlayerImage;

public static class UpdatePlayerImageModule
{
    public static IServiceCollection AddUpdatePlayerImageFeatures(this IServiceCollection services)
    {
        services.AddScoped<UpdatePlayerImageHandler>();
        services.AddScoped<UpdatePlayerImageValidator>();
        
        return services;
    }
}