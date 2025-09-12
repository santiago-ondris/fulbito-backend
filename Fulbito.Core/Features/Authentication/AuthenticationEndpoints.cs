using Fulbito.Core.Features.Authentication.Register;
using Microsoft.AspNetCore.Routing;

namespace Fulbito.Core.Features.Authentication;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRegisterEndpoint();
        app.MapLoginEndpoint();
    }
}