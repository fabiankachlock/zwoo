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

        Mongo.Migration.DocumentVersionSerializer.DefaultVersion = Globals.Version;
        // ZwooDatabase = new Database();
    }

    public static string ConnectionString;
    public static string DatabaseName;
    public static string LogrushDashboardUrl;
    // public static Database ZwooDatabase { set; get; }
    public static readonly string Version = "1.0.0-beta.11";
}