using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabase.Dao;

public class AuditEventDao
{
    public AuditEventDao() { }

    [BsonConstructor]
    public AuditEventDao(ObjectId id, string actor, string message, ulong timestamp, object newValue, object? oldValue)
    {
        Id = id;
        Actor = actor;
        Message = message;
        Timestamp = timestamp;
        NewValue = newValue;
        OldValue = oldValue;
    }

    [BsonElement("_id")]
    public ObjectId Id { get; set; } = new ObjectId();

    [BsonElement("actor")]
    public string Actor { set; get; } = "";

    [BsonElement("message")]
    public string Message { set; get; } = "";

    [BsonElement("timestamp")]
    public ulong Timestamp = 0;

    [BsonElement("newValue")]
    public object NewValue { set; get; } = null!;

    [BsonElement("oldValue")]
    public object? OldValue { set; get; } = null!;
}