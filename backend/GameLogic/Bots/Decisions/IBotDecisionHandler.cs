using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Bots.Decisions;


public interface IBotDecisionHandler
{
    public delegate void EventHandler(ZRPCode code, object payload);
    public event EventHandler OnEvent;
    void AggregateNotification(BotZRPNotification<object> message);
    void Reset();
}