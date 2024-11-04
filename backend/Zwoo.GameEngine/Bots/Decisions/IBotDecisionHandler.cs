using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Bots.Decisions;


public interface IBotDecisionHandler : IDisposable
{
    public delegate void EventHandler(ZRPCode code, object payload);
    public event EventHandler OnEvent;
    void AggregateNotification(BotZRPNotification<object> message);
    void Reset();
}