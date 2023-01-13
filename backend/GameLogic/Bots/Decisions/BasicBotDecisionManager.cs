using ZwooGameLogic.Bots.State;

namespace ZwooGameLogic.Bots.Decisions;


public class BasicBotDecisionManager : IBotDecisionHandler
{
    private BasicBotStateManager _stateManager;

    public BasicBotDecisionManager()
    {
        _stateManager = new BasicBotStateManager();
    }

    public BotZRPNotification<TOut>? AggregateNotification<TIn, TOut>(BotZRPNotification<TIn> message)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}