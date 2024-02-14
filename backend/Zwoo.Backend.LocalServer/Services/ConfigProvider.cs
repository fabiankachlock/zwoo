namespace Zwoo.Backend.LocalServer.Services;


public class ServerConfig
{
    public int Port { get; set; }
    public string IP { get; set; } = string.Empty;
    public bool UseDynamicPort { get; set; }
    public bool UseLocalhost { get; set; }
    public bool UseAllIPs { get; set; }
}

public static class ServerConfigExtensions
{
    private static Dictionary<string, string> argMappings = new Dictionary<string, string>
    {
        {"--port", "server:port"},
        {"--ip", "server:ip"},
        {"--use-dynamic-port", "server:useDynamicPort"},
        {"--use-localhost", "server:useLocalhost"},
        {"--use-all-ips", "server:useAllIPs"},
    };

    public static ServerConfig AddServerConfiguration(this WebApplicationBuilder builder, string[] args)
    {
        IConfigurationRoot config = builder.Configuration
            .AddCommandLine(args, argMappings)
            .Build();

        var value = config.GetSection("server").Get<ServerConfig>();
        if (value == null)
        {
            value = new ServerConfig();
        }
        return value;
    }
}