using Zwoo.GameEngine.ZRP;

namespace Zwoo.Backend.Shared.Api.Game.Adapter;

public struct ZRPMessage : IIncomingZRPMessage
{
    public ZRPCode Code { get; private set; }
    public object? Payload { get; private set; }
    public long UserId { get; private set; }
    public readonly string RawMessage;


    public ZRPMessage(long userId, ZRPCode code, string raw, object? payload = null)
    {
        UserId = userId;
        Code = code;
        Payload = payload;
        RawMessage = raw;
    }

    public T? CastPayload<T>()
    {
        try
        {
            return (T?)Payload;
        }
        catch
        {
            return default(T?);
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
            return default(T?);
        }
    }
}
