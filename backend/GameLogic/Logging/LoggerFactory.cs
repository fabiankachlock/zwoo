namespace ZwooGameLogic.Logging;

public interface ILoggerFactory
{
    public ILogger CreateLogger(string name);
}