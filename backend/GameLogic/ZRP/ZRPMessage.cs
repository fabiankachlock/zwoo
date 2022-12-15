namespace ZwooGameLogic.ZRP;

public interface IZRPMessage
{
    /// <summary>
    /// the operation code of a message
    /// </summary>
    public ZRPCode Code { get; }

    /// <summary>
    /// the parsed payload of a zrp message
    /// 
    /// since the payload gets decoded lazily this will be null until DecodePayload() was called
    /// </summary>
    public object? Payload { get; }

    /// <summary>
    /// the id of the user how sent this message
    /// </summary>
    public long UserId { get; }

    /// <summary>
    /// cast the payload to a certain type
    /// </summary>
    /// <typeparam name="T">the type to cast the payload to</typeparam>
    /// <returns>the casted payload</returns>
    public T? CastPayload<T>();

    /// <summary>
    /// decode the payload to a certain type
    /// </summary>
    /// <typeparam name="T">the type to decode into</typeparam>
    /// <returns>the decoded payload</returns>
    public T? DecodePyload<T>();
}

