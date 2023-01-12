using ZwooGameLogic.Notifications;
using ZwooGameLogic.ZRP;
using ZwooGameLogic.Bots.Decisions;

namespace ZwooGameLogic.Bots;


public class Bot : INotificationTarget
{

    public long GameId { get; private set; }

    public long PlayerId { get; private set; }

    private IBotDecisionHandler _handler;

    private Action<long, BotZRPNotification<object>> _sendMessage;


    public Bot(long gameId, IBotDecisionHandler handler, Action<long, BotZRPNotification<object>> sendMessage)
    {
        GameId = gameId;
        _handler = handler;
        _sendMessage = sendMessage;
    }

    public void PrepareForGame(long assignedId)
    {
        PlayerId = assignedId;
        _handler.Reset();
    }

    public void ReceiveMessage<T>(ZRPCode code, T payload)
    {
        BotZRPNotification<T> msg = new BotZRPNotification<T>(code, payload);
        var botEvent = _handler.AggregateNotification<T, object>(msg);
        if (botEvent != null)
        {
            _sendMessage(PlayerId, botEvent.Value);
        }
    }

    public void ReceiveDisconnect()
    {
        _handler.Reset();
    }
}