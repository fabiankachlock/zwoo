using Zwoo.Api.ZRP;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Bots;

public readonly struct BotZRPNotification<T>
{

    public readonly ZRPCode Code;

    public readonly T Payload;

    public BotZRPNotification(ZRPCode code, T payload)
    {
        Code = code;
        Payload = payload;
    }
}

public readonly struct BotZRPEvent : ILocalZRPMessage
{

    public readonly ZRPCode Code { get; }

    public readonly object Payload { get; }

    public readonly long LobbyId { get; }

    public BotZRPEvent(long botId, ZRPCode code, object payload)
    {
        LobbyId = botId;
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