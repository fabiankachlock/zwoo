using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
using LogRushClient.Log4Net;


namespace ZwooBackend;

public static class Globals
{
    static Globals()
    {
        string GetEnvOrExit(string s)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(s)))
            {
                Console.WriteLine($"{s} is required! please set it as Environment variable");
                Environment.Exit(1);
            }
            return Environment.GetEnvironmentVariable(s)!;
        }

        IsBeta = Convert.ToBoolean(Environment.GetEnvironmentVariable("ZWOO_BETA"));
        UseSsl = Convert.ToBoolean(Environment.GetEnvironmentVariable("USE_SSL"));
        Cors = GetEnvOrExit("ZWOO_CORS");
        ZwooDomain = GetEnvOrExit("ZWOO_DOMAIN");
        ZwooCookieDomain = Environment.GetEnvironmentVariable("ZWOO_COOKIE_DOMAIN") ?? ZwooDomain;

        var extraCors = Environment.GetEnvironmentVariable("ZWOO_CORS_CONTACT_FORM");
        ContactFormExtraCorsOrigin = (extraCors ?? "").Split(',').Append(Cors).Where(host => host != "").ToArray();

        ConnectionString = GetEnvOrExit("ZWOO_DATABASE_CONNECTION_STRING");
        DatabaseName = GetEnvOrExit("ZWOO_DATABASE_NAME");
        SmtpHostUrl = GetEnvOrExit("SMTP_HOST_URL");
        if (!int.TryParse(GetEnvOrExit("SMTP_HOST_PORT").Trim(), out SmtpHostPort))
        {
            Logger.Error($"SMTP_HOST_PORT ({Environment.GetEnvironmentVariable("SMTP_HOST_PORT")}) isn't a Number");
            Environment.Exit(1);
        }
        SmtpHostEmail = GetEnvOrExit("SMTP_HOST_EMAIL");
        SmtpUsername = GetEnvOrExit("SMTP_USERNAME");
        SmtpPassword = GetEnvOrExit("SMTP_PASSWORD");
        SmtpUseSsl = Environment.GetEnvironmentVariable("SMTP_USE_SSL") != "false";
        ContactEmail = GetEnvOrExit("ZWOO_CONTACT_EMAIL");

        RecaptchaSideSecret = GetEnvOrExit("ZWOO_RECAPTCHA_SIDESECRET");
        UseLogRush = Convert.ToBoolean(Environment.GetEnvironmentVariable("ZWOO_USE_LOGRUSH"));
        LogRushUrl = Environment.GetEnvironmentVariable("ZWOO_LOGRUSH_URL") ?? "";
        LogRushAlias = Environment.GetEnvironmentVariable("ZWOO_LOGRUSH_ALIAS") ?? "";
        LogRushId = Environment.GetEnvironmentVariable("ZWOO_LOGRUSH_ID") ?? "";
        LogRushKey = Environment.GetEnvironmentVariable("ZWOO_LOGRUSH_Key") ?? "";

        var versionOverride = Environment.GetEnvironmentVariable("ZWOO_VERSION_OVERRIDE");
        if (versionOverride != null && versionOverride != "")
        {
            ApiVersion = versionOverride;
        }

        Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
        hierarchy.Root.Level = Level.Debug;

        PatternLayout layout = new PatternLayout();
        layout.ConversionPattern = "[%date] [%logger] [%level] %message%newline";
        layout.ActivateOptions();

        ConsoleAppender consoleAppender = new ConsoleAppender();
        consoleAppender.Layout = layout;
        consoleAppender.ActivateOptions();
        hierarchy.Root.AddAppender(consoleAppender);

        if (UseLogRush)
        {
            LogRushLogger logRushAppender = new LogRushLogger
            {
                Layout = layout,
                Alias = LogRushAlias,
                Server = LogRushUrl,
                Key = LogRushKey,
                Id = LogRushId
            };
            logRushAppender.ActivateOptions();
            hierarchy.Root.AddAppender(logRushAppender);
            Console.WriteLine($"using log rush as '{LogRushAlias}'");
        }

        hierarchy.Configured = true;

        Mongo.Migration.DocumentVersionSerializer.DefaultVersion = Globals.Version;
    }

    public static readonly ILog Logger = LogManager.GetLogger("Global");
    public static readonly ILog DatabaseLogger = LogManager.GetLogger("Database");
    public static readonly ILog HttpLogger = LogManager.GetLogger("HTTP");
    public static readonly ILog WsLogger = LogManager.GetLogger("WS");
    public static readonly ILog WebSocketLogger = LogManager.GetLogger("WebSocket");
    public static readonly ILog LobbyLogger = LogManager.GetLogger("Lobby");
    public static readonly ILog GameLogger = LogManager.GetLogger("Game");

    public static readonly bool UseSsl;
    public static readonly bool IsBeta;
    public static readonly string Cors;
    public static readonly string[] ContactFormExtraCorsOrigin;
    public static readonly string ZwooDomain;
    public static readonly string ZwooCookieDomain;

    public static readonly string ConnectionString;
    public static readonly string DatabaseName;
    public static readonly string SmtpHostUrl;
    public static readonly int SmtpHostPort;
    public static readonly string SmtpHostEmail;
    public static readonly string SmtpUsername;
    public static readonly string SmtpPassword;
    public static readonly bool SmtpUseSsl;
    public static readonly string ContactEmail;

    public static readonly bool UseLogRush;
    public static readonly string LogRushUrl;
    public static readonly string LogRushId;
    public static readonly string LogRushKey;
    public static readonly string LogRushAlias;


    public static readonly string RecaptchaSideSecret;

    public static readonly string Version = "1.0.0-beta.14";
    public static readonly string ApiVersion = Version;
}
