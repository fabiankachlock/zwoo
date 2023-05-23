using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;

namespace ZwooInfoDashBoard.Data;

public static class Globals
{
    static Globals()
    {
        string ReturnIfValidEnvVar(string s)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(s)))
            {
                Console.WriteLine($"{s} is required! please set it as Environment variable");
                Environment.Exit(1);
            }
            return Environment.GetEnvironmentVariable(s)!;
        }


        ConnectionString = ReturnIfValidEnvVar("ZWOO_DATABASE_CONNECTION_STRING");
        DatabaseName = ReturnIfValidEnvVar("ZWOO_DATABASE_NAME");
        LogrushDashboardUrl = Environment.GetEnvironmentVariable("LOGRUSH_DASHBOARD") ?? "";

        AuthenticationAuthority = ReturnIfValidEnvVar("ZWOO_AUTH_AUTHORITY");
        AuthenticationClientId = ReturnIfValidEnvVar("ZWOO_AUTH_CLIENT_ID");
        AuthenticationClientSecret = ReturnIfValidEnvVar("ZWOO_AUTH_CLIENT_SECRET");
        AuthenticationRole = ReturnIfValidEnvVar("ZWOO_AUTH_ROLE");

        Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
        hierarchy.Root.Level = Level.Debug;

        PatternLayout layout = new PatternLayout();
        layout.ConversionPattern = "[%date] [%logger] [%level] %message%newline";
        layout.ActivateOptions();

        ConsoleAppender consoleAppender = new ConsoleAppender();
        consoleAppender.Layout = layout;
        consoleAppender.ActivateOptions();
        hierarchy.Root.AddAppender(consoleAppender);

        hierarchy.Configured = true;

        Mongo.Migration.DocumentVersionSerializer.DefaultVersion = Globals.Version;
    }

    public static string AuthenticationAuthority;
    public static string AuthenticationClientId;
    public static string AuthenticationClientSecret;
    public static string AuthenticationRole;

    public static string ConnectionString;
    public static string DatabaseName;
    public static string LogrushDashboardUrl;
    public static readonly string Version = "1.0.0-beta.11";
}