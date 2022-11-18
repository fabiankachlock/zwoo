using System.Text.Json.Serialization;
using Mongo.Migration;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseClasses;

[RuntimeVersion("1.0.0-beta.7")]
[StartUpVersion("1.0.0-beta.7")]
[CollectionLocation("changelogs", "zwoo")]
public class GameInfo : IDocument
{
    public GameInfo() {}
    
    public GameInfo(string gameName, long gameId, bool isPublic, List<PlayerScore> scores, ulong timeStamp)
    {
        GameName = gameName;
        GameId = gameId;
        IsPublic = isPublic;
        Scores = scores;
        TimeStamp = timeStamp;
    }
    
    [BsonConstructor]
    public GameInfo(ObjectId id, string gameName, long gameId, bool isPublic, List<PlayerScore> scores, ulong timeStamp)
    {
        Id = id;
        GameName = gameName;
        GameId = gameId;
        IsPublic = isPublic;
        Scores = scores;
        TimeStamp = timeStamp;
    }
    
    [BsonElement("_id")]
    public ObjectId Id { set; get; }
    
    [BsonElement("name")]
    public string GameName { set; get; } = "";
    [BsonElement("game_id")]
    public long GameId { set; get; } = 0;
    [BsonElement("is_public")]
    public bool IsPublic { set; get; } = false;
    [BsonElement("scores")]
    public List<PlayerScore> Scores = null!;
    [BsonElement("timestamp")]
    public ulong TimeStamp { set; get; } = 0;
    
    [BsonIgnore]
    public string Winner => Scores.First(x => x.Score == 0).PlayerUsername;

    [JsonIgnore]
    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}

public class PlayerScore
{
    public PlayerScore() {}
    
    [BsonConstructor]
    public PlayerScore(string playerUsername, int score)
    {
        PlayerUsername = playerUsername;
        Score = score;
    }

    [BsonElement("player_username")]
    public string PlayerUsername { set; get; } = "";

    [BsonElement("score")]
    public int Score { set; get; } = 0;
    
}