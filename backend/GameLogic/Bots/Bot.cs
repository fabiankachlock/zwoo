using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Bots.State;

namespace ZwooGameLogic.Bots;


public class Bot : INotificationTarget
{

    public long GameId { get; private set; }

    public long PlayerId { get; private set; }

    private BotStateManager _stateManager;


    public Bot(long gameId)
    {
        GameId = gameId;
        _stateManager = new BotStateManager();
    }

    public void PrepareForGame(long assignedId)
    {
        PlayerId = assignedId;
        _stateManager.Reset();
    }

    public void ReceiveMessage<T>(ZRPCode code, T payload)
    {
        // detect if update or not
        // send to state manager
        // or make decision
    }

    public void ReceiveDisconnect()
    {
        // EOL
    }
}