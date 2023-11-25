using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zwoo.Backend.Shared.Services;

public class HttpLogger { }

public static class LoggingExtensions
{
    public static void AddZwooLogging(this IServiceCollection services)
    {
        services.AddLogging(options =>
        {
            options.AddSimpleConsole(c =>
            {
                c.SingleLine = true;
                c.UseUtcTimestamp = true;
                c.TimestampFormat = "[yyyy-MM-ddTHH:mm:ss] ";
            });
        });
    }

    public static void UseZwooHttpLogging(this WebApplication app)
    {
        ILogger<HttpLogger> logger = app.Services.GetService<ILogger<HttpLogger>>()!;
        app.Use(async (context, next) =>
        {
            var watch = new Stopwatch();
            var log = "";
            if (context.Request.Method != "OPTIONS")
            {
                watch.Start();
                log += $"{context.Connection.RemoteIpAddress} {context.Request.Protocol} {context.Request.Path}{context.Request.QueryString}";
            }
            await next.Invoke();
            if (context.Response.StatusCode >= 300)
            {
                log += $" sending error response: {context.Response.StatusCode}";
            }
            if (context.Request.Method != "OPTIONS")
            {
                watch.Stop();
                log += $" ({watch.ElapsedMilliseconds}ms)";
            }
            logger.LogInformation(log);
        });
    }
}