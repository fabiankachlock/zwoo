using System.Collections.Concurrent;
using BackendHelper;
using log4net;

namespace ZwooBackend;

public static class Globals
{
    public static ILog Logger =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

    public static Database.Database ZwooDatabase = new Database.Database();

    public static ConcurrentQueue<EmailData> EmailQueue = new ConcurrentQueue<EmailData>(); // use BlockingCollection here?

    public static bool IsBeta = Convert.ToBoolean(Environment.GetEnvironmentVariable("ZWOO_BETA"));
}
