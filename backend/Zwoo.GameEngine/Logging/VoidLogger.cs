namespace Zwoo.GameEngine.Logging;

public sealed class VoidLogger : ILogger
{
    public void Debug(string msg) { }

    public void Error(string msg) { }

    public void Info(string msg) { }

    public void Warn(string msg) { }
}