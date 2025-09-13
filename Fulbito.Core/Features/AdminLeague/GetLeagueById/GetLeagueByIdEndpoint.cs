using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.AdminLeague.GetLeagueById;

public static class GetLeagueByIdEndpoint
{
    public static void MapGetLeagueByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/leagues/{id:guid}", async (
            Guid id,
            GetLeagueByIdHandler handler) =>
        {
            var query = new GetLeagueByIdQuery(id);
            var result = await handler.Handle(query);
            
            return result != null 
                ? Results.Ok(result)
                : Results.NotFound(new { message = "Liga no encontrada o no autorizada" });
        })
        .RequireAuthorization(); // Requiere JWT token
    }
}