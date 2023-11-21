using log4net;

namespace Zwoo.Backend.Games;

public class Log4NetFactory : Zwoo.GameEngine.Logging.ILoggerFactory
{
    public Zwoo.GameEngine.Logging.ILogger CreateLogger(string name)
    {
        return new Log4NetLogger(name);
    }
}


public class Log4NetLogger : Zwoo.GameEngine.Logging.ILogger
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
