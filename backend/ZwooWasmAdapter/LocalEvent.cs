using ZwooGameLogic.ZRP;

namespace ZwooWasm;


public class LocalEvent : IIncomingZRPMessage
{

    public ZRPCode Code { get; private set; }

    public object Payload { get; private set; }

    public long UserId => Constants.LocalUser;

    public LocalEvent(ZRPCode code, object payload)
    {
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