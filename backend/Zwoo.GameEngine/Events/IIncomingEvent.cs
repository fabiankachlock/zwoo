using Zwoo.GameEngine.ZRP;

namespace Zwoo.GameEngine.Events;

public interface IIncomingEvent
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
    /// the raw zrp message
    /// </summary>
    public string RawMessage { get; }

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
    public T? DecodePayload<T>();
}