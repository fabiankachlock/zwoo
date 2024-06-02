namespace Zwoo.Backend.LocalServer.Services;


public class ServerConfig
{
    public int Port { get; set; }
    public string IP { get; set; } = string.Empty;
    public bool UseDynamicPort { get; set; }
    public bool UseLocalhost { get; set; }
    public bool UseAllIPs { get; set; }
    public string ServerId { get; set; } = "server"; // TODO: testing only
    public string SecretKey { get; set; } = string.Empty;
    public bool UseStrictOrigins { get; set; }
    public string AllowedOrigins { get; set; } = string.Empty;
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
        {"--server-id", "server:serverId"},
        {"--secret-key", "server:secretKey"},
        {"--strict-origins", "server:useStrictOrigins"},
        {"--allowed-origins", "server:allowedOrigins"},
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

    public static void PrintHelp(this ServerConfig config)
    {
        Console.WriteLine("Welcome to the local server of zwoo!");
        Console.WriteLine("USAGE:");
        Console.WriteLine("  -h                            Print this help");
        Console.WriteLine("  --server-id <id>              Server ID (used for cookies)");
        Console.WriteLine("  --secret-key <id>             Servers secret key (used for the stats endpoints)");
        Console.WriteLine("  --port <port>                 Port to listen on");
        Console.WriteLine("  --ip <ip>                     IP to listen on");
        Console.WriteLine("  --use-dynamic-port            Use a dynamic port");
        Console.WriteLine("  --use-localhost               Listen on localhost");
        Console.WriteLine("  --use-all-ips                 Listen on all IPs");
        Console.WriteLine("  --strict-origins              Allow only access via built in website");
        Console.WriteLine("  --allowed-origins <origins>   Allowed origins");
    }
}