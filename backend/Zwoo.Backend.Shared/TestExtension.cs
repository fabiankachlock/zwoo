using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Zwoo.Backend.Shared;

public class VersionInfo
{
    public required string Version { get; set; }
    public required string ZRPVersion { get; set; }
}

public static class AppExtensions
{
    public static void UseDiscover(this WebApplication app)
    {
        app.MapGet("/server/discover", () =>
        {
            return Results.Ok(new VersionInfo()
            {
                Version = "a",
                ZRPVersion = "b"
            });
        })
            .WithName("discover")
            .WithOpenApi();
    }
}