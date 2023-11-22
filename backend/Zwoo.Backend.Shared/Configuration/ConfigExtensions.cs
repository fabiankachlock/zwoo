using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Zwoo.Backend.Shared.Configuration;

public static class ZwooConfigurationExtensions
{
    private static Dictionary<string, string> argMappings = new Dictionary<string, string>
    {
        {"--db-uri", "zwoo:database:connectionUri"},
        {"--db-name", "zwoo:database:dbname"},
        {"--email-host", "zwoo:email:host"},
        {"--email-port", "zwoo:email:port"},
        {"--email-address", "zwoo:email:email"},
        {"--email-username", "zwoo:email:username"},
        {"--email-password", "zwoo:email:password"},
        {"--email-use-ssl", "zwoo:email:useSsl"},
        {"--email-contact-email", "zwoo:email:contactEmail"},
        {"--features-beta", "zwoo:features:isBeta"},
        {"--features-captcha-secret", "zwoo:features:captchaSecret"},
        {"--server-use-ssl", "zwoo:server:useSsl"},
        {"--server-cors", "zwoo:server:cors"},
        {"--server-contact-origins", "zwoo:server:contactFormExtraCorsOrigin"},
        {"--server-domain", "zwoo:server:domain"},
        {"--server-cookie-domain", "zwoo:server:cookieDomain"},
    };

    public static ZwooOptions AddZwooConfiguration(this WebApplicationBuilder builder, string[] args, ZwooAppConfiguration staticConfig)
    {
        IConfigurationRoot config = builder.Configuration
            .AddJsonFile("zwoo.json", optional: true)
            .AddEnvironmentVariables("ZWOO_")
            .AddCommandLine(args, argMappings)
            .Build();

        Console.WriteLine(config.GetDebugView());
        var value = config.GetSection("ZWOO").Get<ZwooOptions>();
        value!.App = staticConfig;
        builder.Services.Configure<ZwooOptions>(config.GetSection("ZWOO"));
        return value;
    }
}