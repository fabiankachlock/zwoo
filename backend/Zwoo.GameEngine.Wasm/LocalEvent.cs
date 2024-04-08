using Zwoo.GameEngine.ZRP;

namespace ZwooWasm;


public class LocalEvent : IIncomingZRPMessage
{

    public ZRPCode Code { get; }

    public object? Payload { get; private set; }

    public long UserId => Constants.LocalUser;

    public string RawMessage { get; }

    public LocalEvent(ZRPCode code, string message)
    {
        Code = code;
        RawMessage = message;
    }

    public T? CastPayload<T>()
    {
        try
        {
            return (T?)Payload;
        }
        catch
        {
            return default;
        }
    }

    public T? DecodePayload<T>()
    {
        try
        {
            return ZRPDecoder.DecodePayload<T>(RawMessage);
        }
        catch
        {
            return default;
        }
    }
}