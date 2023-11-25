using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zwoo.Backend.Shared.Services;

public static class LoggingExtensions
{
    public static void AddZwooLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(options =>
        {
            options.AddSimpleConsole(c =>
            {
                c.SingleLine = true;
                c.UseUtcTimestamp = true;
                c.TimestampFormat = "[yyyy-MM-ddTHH:mm:ss] ";
            });
        });
    }
}