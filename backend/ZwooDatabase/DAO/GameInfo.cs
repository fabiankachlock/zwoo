using Mongo.Migration;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabase.Dao;

[RuntimeVersion("1.0.0-beta.8")]
[StartUpVersion("1.0.0-beta.8")]
[CollectionLocation("game_info")]
public class GameInfoDao : IDocument
{
    public GameInfoDao() { }

    public GameInfoDao(string gameName, long gameId, bool isPublic, List<PlayerScoreDao> scores, ulong timeStamp)
    {
        GameName = gameName;
        GameId = gameId;
        IsPublic = isPublic;
        Scores = scores;
        TimeStamp = timeStamp;
    }

    [BsonConstructor]
    public GameInfoDao(ObjectId id, string gameName, long gameId, bool isPublic, List<PlayerScoreDao> scores, ulong timeStamp)
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
    public List<PlayerScoreDao> Scores { get; set; } = null!;

    /// <summary>
    /// time in unix seconds
    /// </summary>
    [BsonElement("timestamp")]
    public ulong TimeStamp { set; get; } = 0;

    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}