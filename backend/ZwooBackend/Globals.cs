using log4net;

namespace ZwooBackend;

public static class Globals
{
    public static ILog Logger =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

    public static Database.Database ZwooDatabase = new Database.Database();
}
