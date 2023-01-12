using ZwooGameLogic.ZRP;

namespace ZwooGameLogic.Bots;

public struct BotZRPNotification<T>
{

    public readonly ZRPCode Code;

    public readonly T Payload;

    public BotZRPNotification(ZRPCode code, T payload)
    {
        Code = code;
        Payload = payload;
    }

}

public class BotZRPEvent : IIncomingZRPMessage
{

    public ZRPCode Code { get; private set; }

    public object Payload { get; private set; }

    public long UserId { get; private set; }

    public BotZRPEvent(long botId, ZRPCode code, object payload)
    {
        UserId = botId;
        Code = code;
        Payload = payload;
    }

    public T? CastPayload<T>()
    {
        return (T)Payload;
    }

    public T? DecodePayload<T>()
    {
        return (T)Payload;
    }
}