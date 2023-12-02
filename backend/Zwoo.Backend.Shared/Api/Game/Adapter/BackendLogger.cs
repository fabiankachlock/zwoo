using Microsoft.Extensions.Logging;

namespace Zwoo.Backend.Games;

public class BackendLoggerFactory : GameEngine.Logging.ILoggerFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public BackendLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public GameEngine.Logging.ILogger CreateLogger(string name)
    {
        return new GameEngineLogger(_loggerFactory.CreateLogger<GameEngineLogger>(), name);
    }
}


public class GameEngineLogger : GameEngine.Logging.ILogger
{
    private readonly ILogger<GameEngineLogger> _logger;
    private readonly string _prefix;

    public GameEngineLogger(ILogger<GameEngineLogger> logger, string name)
    {
        _logger = logger;
        _prefix = $"[{name}]";
    }

    public void Debug(string msg)
    {
        _logger.LogDebug($"{_prefix} {msg}");
    }

    public void Error(string msg)
    {
        _logger.LogError($"{_prefix} {msg}");
    }

    public void Info(string msg)
    {
        _logger.LogInformation($"{_prefix} {msg}");
    }

    public void Warn(string msg)
    {
        _logger.LogWarning($"{_prefix} {msg}");
    }
}
