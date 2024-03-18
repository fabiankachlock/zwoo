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
    public static void AddZwooLogging(this WebApplicationBuilder builder, bool useFile)
    {
        builder.Services.AddLogging(options =>
        {
            options.AddSimpleConsole(c =>
            {
                // c.SingleLine = true;
                c.UseUtcTimestamp = true;
                c.TimestampFormat = "[yyyy-MM-ddTHH:mm:ss] ";
            });
            if (useFile)
            {
                // options.AddFile(builder.Configuration.GetSection("Logging"));
            }
        });
    }

    public static void UseZwooHttpLogging(this WebApplication app, string subPath = "")
    {
        ILogger<HttpLogger> logger = app.Services.GetService<ILogger<HttpLogger>>()!;
        app.Use(async (context, next) =>
        {
            if (!context.Request.Path.StartsWithSegments(subPath))
            {
                await next(context);
                return;
            }

            var watch = new Stopwatch();
            var log = "";
            var warning = false;
            if (context.Request.Method != "OPTIONS")
            {
                log += $"{context.Connection.RemoteIpAddress} {context.Request.Protocol} {context.Request.Path}{context.Request.QueryString}";
                if (context.WebSockets.IsWebSocketRequest)
                {
                    log += " (websocket)";
                    logger.LogInformation(log);
                    try
                    {
                        await next(context);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"internal error occurred while processing request {context.Request.Path} of {context.TraceIdentifier}");
                    }
                    Console.WriteLine("!!! after ws logger");
                    return;
                }
                else
                {
                    watch.Start();
                }
            }


            try
            {
                await next(context);
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