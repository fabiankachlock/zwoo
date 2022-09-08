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
        LogrushDashboardUrl = Environment.GetEnvironmentVariable("LOGRUSH_DASHBOARD") ?? "";
        ZwooDatabase = new Database();
    }

    public static string ConnectionString;
    public static string LogrushDashboardUrl;
    public static Database ZwooDatabase { set; get; }
}