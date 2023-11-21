namespace Zwoo.GameEngine.Logging;

public interface ILogger
{
    public void Debug(string msg);
    public void Info(string msg);
    public void Warn(string msg);
    public void Error(string msg);

}