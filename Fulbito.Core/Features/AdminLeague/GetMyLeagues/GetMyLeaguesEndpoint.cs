using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.AdminLeague.GetMyLeagues;

public static class GetMyLeaguesEndpoint
{
    public static void MapGetMyLeaguesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/my-leagues", async (GetMyLeaguesHandler handler) =>
        {
            var query = new GetMyLeaguesQuery();
            var result = await handler.Handle(query);
            
            return result != null 
                ? Results.Ok(result)
                : Results.Unauthorized();
        })
        .RequireAuthorization();
    }
}