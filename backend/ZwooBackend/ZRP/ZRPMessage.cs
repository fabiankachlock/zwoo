namespace ZwooBackend.ZRP;

public readonly struct ZRPMessage
{
    public readonly ZRPCode Code;
    public readonly object? Payload;
    public readonly string RawMessage;

    public ZRPMessage(ZRPCode code, string raw, object? payload = null)
    {
        Code = code;
        Payload = payload;
        RawMessage = raw;
    }

    public T? CastPayload<T>()
    {
        return (T?)Payload;
    }

    public T? DecodePyload<T>()
    {
        return ZRPDecoder.DecodePayload<T>(RawMessage);
    }
}
