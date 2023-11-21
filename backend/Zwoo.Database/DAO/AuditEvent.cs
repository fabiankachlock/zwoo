using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zwoo.Database.Dao;

public class AuditEventDao
{
    public AuditEventDao()
    {
        Id = ObjectId.GenerateNewId();
        Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public AuditEventDao(string actor, string message, long timestamp, object newValue, object? oldValue)
    {
        Id = ObjectId.GenerateNewId();
        Actor = actor;
        Message = message;
        Timestamp = timestamp;
        NewValue = newValue;
        OldValue = oldValue;
    }

    [BsonConstructor]
    public AuditEventDao(ObjectId id, string actor, string message, long timestamp, object newValue, object? oldValue)
    {
        Id = id;
        Actor = actor;
        Message = message;
        Timestamp = timestamp;
        NewValue = newValue;
        OldValue = oldValue;
    }

    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("actor")]
    public string Actor { set; get; } = "";

    [BsonElement("message")]
    public string Message { set; get; } = "";

    /// <summary>
    /// time in unix milliseconds
    /// </summary>
    [BsonElement("timestamp")]
    public long Timestamp { get; set; }

    [BsonElement("newValue")]
    public object NewValue { set; get; } = null!;

    [BsonElement("oldValue")]
    public object? OldValue { set; get; } = null!;
}