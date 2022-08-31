using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooBackend.Database;

public class AccountEvent
{
    public AccountEvent() {}
    
    [BsonConstructor]
    public AccountEvent(string type, ulong playerId, bool success, ulong timeStamp)
    {
        EventType = type;
        PlayerID = playerId;
        Success = success;
        TimeStamp = timeStamp;
    }
    
    [BsonRepresentation(BsonType.String)]
    [BsonElement("event_type")]
    public string EventType { set; get; } = "none";

    [BsonRepresentation(BsonType.Int64)]
    [BsonElement("player_id")]
    public ulong PlayerID { set; get; } = 0;
    
    [BsonRepresentation(BsonType.Boolean)]
    [BsonElement("success")]
    public bool Success;
    
    [BsonRepresentation(BsonType.Timestamp)]
    [BsonElement("timestamp")]
    public ulong TimeStamp;
}