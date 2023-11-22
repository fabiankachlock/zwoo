using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Shared.Api.Model;

namespace Zwoo.Backend.Shared.Api.Discover;

public static class DiscoverEndpoint
{
    public static void Map(this WebApplication app)
    {
        app.MapPost("/server/discover", ([FromBody] ClientInfo client, IDiscoverService _service) =>
        {
            if (_service.CanConnect(client))
            {
                return Results.BadRequest(new ProblemDetails());
            }
            return Results.Ok();
        })
            .WithName("discover")
            .AllowAnonymous();
    }
}