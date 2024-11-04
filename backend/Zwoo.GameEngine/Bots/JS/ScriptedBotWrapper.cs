using Microsoft.ClearScript;

namespace Zwoo.GameEngine.Bots.JS;

internal class ScriptedBotWrapper : ScriptedBot
{
    private ScriptObject _host;

    public ScriptedBotWrapper(ScriptObject host)
    {
        _host = host;
    }

    public void AggregateNotification(BotZRPNotification<object> message)
    {
        _host.InvokeMethod("AggregateNotification", message);
    }

    public void Reset()
    {
        _host.InvokeMethod("Reset");
    }
}
