using Fulbito.Core.Features.AdminLeague.GetLeagueById;
using Fulbito.Core.Features.AdminLeague.GetMyLeagues;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.AdminLeague;

public static class AdminLeagueEndpoints
{
    public static void MapAdminLeagueEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetLeagueByIdEndpoint();
        app.MapGetMyLeaguesEndpoint();
    }
}