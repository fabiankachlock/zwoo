using Mongo.Migration;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseClasses;

[RuntimeVersion("1.0.0-beta.7")]
[StartUpVersion("1.0.0-beta.7")]
[CollectionLocation("account_events")]
public class AccountEvent : IDocument
{
    public AccountEvent() { }

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

    [BsonElement("user_data")]
    public DeletedUserData? UserData;

    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}

public class DeletedUserData
{
    public DeletedUserData() { }

    public DeletedUserData(User? user)
    {
        if (user == null)
            return;
        Username = user.Username;
        Email = user.Email;
        Password = user.Password;
        Wins = user.Wins;
        Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    [BsonConstructor]
    public DeletedUserData(string username, string email, string password, uint wins, ulong timestamp)
    {
        Username = username;
        Email = email;
        Password = password;
        Wins = wins;
        Timestamp = timestamp;
    }

    [BsonElement("username")]
    public string Username { set; get; } = "";

    [BsonElement("email")]
    public string Email { set; get; } = "";

    [BsonElement("password")]
    public string Password { set; get; } = "";

    [BsonElement("wins")]
    public UInt32 Wins { set; get; } = 0;

    [BsonElement("timestamp")]
    public ulong Timestamp;
}