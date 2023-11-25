using System.Security.Principal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Shared.Api.Model;

namespace Zwoo.Backend.Shared.Api.Discover;

public static class DiscoverEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/discover", ([FromBody] ClientInfo client, IDiscoverService _service, HttpContext context) =>
        {
            if (_service.CanConnect(client))
            {
                return Results.BadRequest(ApiError.Example.ToProblem(new ProblemDetails()));
            }
            return Results.Ok();
        });
    }
}