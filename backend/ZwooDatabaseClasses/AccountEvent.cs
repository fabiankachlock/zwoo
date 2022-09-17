using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseClasses;

public class AccountEvent
{
    public AccountEvent() {}

    public AccountEvent(string eventType, ulong playerId, bool success, ulong timeStamp)
    {
        EventType = eventType;
        PlayerID = playerId;
        Success = success;
        TimeStamp = timeStamp;
    }
    
    [BsonConstructor]
    public AccountEvent(ObjectId id, string eventType, ulong playerId, bool success, ulong timeStamp)
    {
        Id = id;
        EventType = eventType;
        PlayerID = playerId;
        Success = success;
        TimeStamp = timeStamp;
    }
    
    [BsonElement("_id")]
    public ObjectId Id { set; get; }
    
    [BsonElement("event_type")]
    public string EventType { set; get; } = "none";

    [BsonElement("player_id")]
    public ulong PlayerID { set; get; } = 0;
    
    [BsonElement("success")]
    public bool Success;
    
    [BsonElement("timestamp")]
    public ulong TimeStamp;
}