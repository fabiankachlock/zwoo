using Mongo.Migration;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabase.Dao;

[RuntimeVersion("1.0.0-beta.7")]
[StartUpVersion("1.0.0-beta.7")]
[CollectionLocation("account_events")]
public class AccountEventDao : IDocument
{
    public AccountEventDao() { }

    public AccountEventDao(string eventType, ulong playerId, bool success, ulong timeStamp)
    {
        EventType = eventType;
        PlayerID = playerId;
        Success = success;
        TimeStamp = timeStamp;
    }

    [BsonConstructor]
    public AccountEventDao(ObjectId id, string eventType, ulong playerId, bool success, ulong timeStamp)
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

    [BsonElement("user_data")]
    public DeletedUserDao? UserData;

    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}