using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zwoo.Backend.Shared.Services;

public class HttpLogger { }

enum RollingInterval
{
    Day = 1
}

public static class LoggingExtensions
{
    public static void AddZwooLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(options =>
        {
            options.AddSimpleConsole(c =>
            {
                // c.SingleLine = true;
                c.UseUtcTimestamp = true;
                c.TimestampFormat = "[yyyy-MM-ddTHH:mm:ss] ";
            });
            options.AddFile(builder.Configuration.GetSection("Logging"));
        });
    }

    public static void UseZwooHttpLogging(this WebApplication app)
    {
        ILogger<HttpLogger> logger = app.Services.GetService<ILogger<HttpLogger>>()!;
        app.Use(async (context, next) =>
        {
            var watch = new Stopwatch();
            var log = "";
            var warning = false;
            if (context.Request.Method != "OPTIONS")
            {
                watch.Start();
                log += $"{context.Connection.RemoteIpAddress} {context.Request.Protocol} {context.Request.Path}{context.Request.QueryString}";
            }

            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"internal error occurred while processing request {context.Request.Path} of {context.TraceIdentifier}");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new ProblemDetails()
                {
                    Title = "shit happened",
                    Detail = "shit happened",
                    Status = 500,
                    Instance = context.Request.Path,
                    Type = "example.com/internal-server-error",
                }, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }, "application/problem+json; charset=utf-8");
            }

            if (context.Response.StatusCode >= 400)
            {
                log += $" sending error response: {context.Response.StatusCode}";
                warning = true;
            }
            if (context.Request.Method != "OPTIONS")
            {
                watch.Stop();
                log += $" ({watch.ElapsedMilliseconds}ms)";
            }

            if (log.Count() == 0) return;
            if (warning)
            {
                logger.LogWarning(log);
            }
            else
            {
                logger.LogInformation(log);
            }
        });
    }
}