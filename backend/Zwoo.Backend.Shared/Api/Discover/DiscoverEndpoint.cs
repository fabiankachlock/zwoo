using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Zwoo.Backend.Shared.Api.Model;

namespace Zwoo.Backend.Shared.Api.Discover;

public static class DiscoverEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/discover", ([FromBody] ClientInfo client, IDiscoverService _service, HttpContext context) =>
        {
            ClientInfo value = _service.GetVersion();
            if (!_service.CanConnect(client))
            {
                return Results.BadRequest(ApiError.InvalidClient.ToProblem(new ProblemDetails()
                {
                    Title = "Cant discover this server",
                    Detail = "The versions of this client are incompatible with this server",
                    Instance = "/discover",
                    Extensions = new Dictionary<string, object?>()
                    {
                        {"version", value.Version},
                        {"zrpVersion", value.ZRPVersion},
                    }.ToDictionary()
                }));
            }
            return Results.Ok(value);
        }).AllowAnonymous();
    }
}