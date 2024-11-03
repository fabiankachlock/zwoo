using Microsoft.ClearScript;

namespace Zwoo.GameEngine.Bots.JS;


/// <summary>
/// The interface of the bot inside javascript
/// </summary>
internal interface ScriptedBot
{
    void AggregateNotification(BotZRPNotification<object> message);
    void Reset();
}