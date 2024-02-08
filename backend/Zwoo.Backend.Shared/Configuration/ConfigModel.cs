namespace Zwoo.Backend.Shared.Configuration;

public class ZwooAppConfiguration
{
    public string AppVersion { get; set; } = "";
    public string ZRPVersion { get; set; } = "";
}

public class EmailOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public bool UseSsl { get; set; }
    public string ContactEmail { get; set; } = "";
}

public class DatabaseOptions
{
    public string ConnectionUri { get; set; } = "";
    public string DBName { get; set; } = "";
}

public class ZwooFeatures
{
    public bool IsBeta { get; set; }
    public string CaptchaSecret { get; set; } = "";
}

public class ServerOptions
{
    public bool UseSsl { get; set; }
    public string Cors { get; set; } = "";
    public string[] ContactFormExtraCorsOrigin { get; set; } = [];
    public string Domain { get; set; } = "";
    public string CookieDomain { get; set; } = "";
}

public class ZwooOptions
{
    public DatabaseOptions Database { get; set; } = new();
    public EmailOptions Email { get; set; } = new();
    public ServerOptions Server { get; set; } = new();
    public ZwooFeatures Features { get; set; } = new();
    public ZwooAppConfiguration App { get; set; } = new();
}