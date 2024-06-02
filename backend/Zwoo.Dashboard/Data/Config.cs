namespace Zwoo.Dashboard.Data;

public class AuthOptions
{
    public string Authority { get; set; } = "";
    public string Role { get; set; } = "";
    public string ClientID { get; set; } = "";
    public string ClientSecret { get; set; } = "";
}

public class ZiadAppConfiguration
{
    public string AppVersion { get; set; } = "";
}

public class ZiadOptions
{
    public AuthOptions Auth { get; set; } = new();
    public ZiadAppConfiguration App { get; set; } = new();
}

public static class ZwooConfigurationExtensions
{
    private static Dictionary<string, string> argMappings = new Dictionary<string, string>
    {
        {"--auth-authority", "ziad:auth:authority"},
        {"--auth-role", "ziad:auth:role"},
        {"--auth-client-id", "ziad:auth:clientId"},
        {"--auth-client-secret", "ziad:auth:clientSecret"},
    };

    /// <summary>
    /// add the default ziad configuration mechanism to the api
    /// </summary>
    /// <param name="builder">the web application builder</param>
    public static ZiadOptions AddZiadConfiguration(this WebApplicationBuilder builder, string[] args, ZiadAppConfiguration staticConfig)
    {
        IConfigurationRoot config = builder.Configuration
            .AddJsonFile("zwoo.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args, argMappings)
            .Build();

        var value = config.GetSection("ZIAD").Get<ZiadOptions>();
        if (value == null)
        {
            value = new ZiadOptions();
        }
        value.App = staticConfig;
        builder.Services.Configure<ZiadOptions>(config.GetSection("ZIAD"));
        builder.Services.AddSingleton(value);
        return value;
    }
}