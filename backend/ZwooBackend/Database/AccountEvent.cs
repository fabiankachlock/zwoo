using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooBackend.Database;

public class AccountEvent
{
    public AccountEvent() {}
    
    [BsonConstructor]
    public AccountEvent(string eventType, ulong playerId, bool success, ulong timeStamp)
    {
        EventType = eventType;
        PlayerID = playerId;
        Success = success;
        TimeStamp = timeStamp;
    }
    
    [BsonElement("event_type")]
    public string EventType { set; get; } = "none";

    [BsonElement("player_id")]
    public ulong PlayerID { set; get; } = 0;
    
    [BsonElement("success")]
    public bool Success;
    
    [BsonElement("timestamp")]
    public ulong TimeStamp;
}