using log4net;

namespace ZwooBackend.Games;

public class Log4NetFactory : ZwooGameLogic.Logging.ILoggerFactory
{
    public ZwooGameLogic.Logging.ILogger CreateLogger(string name)
    {
        return new Log4NetLogger(name);
    }
}


public class Log4NetLogger : ZwooGameLogic.Logging.ILogger
{
    private readonly ILog _logger;

    public Log4NetLogger(string name)
    {
        _logger = LogManager.GetLogger(name);
    }

    public void Debug(string msg)
    {
        _logger.Debug(msg);
    }

    public void Error(string msg)
    {
        _logger.Error(msg);
    }

    public void Info(string msg)
    {
        _logger.Info(msg);
    }

    public void Warn(string msg)
    {
        _logger.Warn(msg);
    }
}
