namespace Zwoo.GameEngine.Logging;

public interface ILoggerFactory
{
    public ILogger CreateLogger(string name);
}