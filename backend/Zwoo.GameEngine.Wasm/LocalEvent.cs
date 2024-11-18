using Zwoo.Api.ZRP;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Wasm;


public class LocalEvent : IIncomingZRPMessage
{

    public ZRPCode Code { get; private set; }

    public object? Payload { get; private set; }

    public long UserId => Constants.LocalUser;

    private string _rawMessage;

    public LocalEvent(ZRPCode code, string message)
    {
        Code = code;
        _rawMessage = message;
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
            return ZRPDecoder.DecodePayload<T>(_rawMessage);
        }
        catch
        {
            return default;
        }
    }
}