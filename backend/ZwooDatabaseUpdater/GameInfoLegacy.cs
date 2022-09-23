using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseUpdater;

public class GameInfoLegacy
{
    public GameInfoLegacy() {}
    
    [BsonConstructor]
    public GameInfoLegacy(ObjectId id, string gameName, long gameId, bool isPublic, List<PlayerScoreLegacy> scores, ulong timeStamp)
    {
        Id = id;
        GameName = gameName;
        GameID = gameId;
        IsPublic = isPublic;
        Scores = scores;
        TimeStamp = timeStamp;
    }
    
    [BsonElement("_id")]
    public ObjectId Id { set; get; }
    
    [BsonElement("name")]
    public string GameName { set; get; } = "";
    [BsonElement("game_id")]
    public long GameID { set; get; } = 0;
    [BsonRepresentation(BsonType.Boolean)]
    [BsonElement("is_public")]
    public bool IsPublic { set; get; } = false;
    /// <summary>
    /// PlayerID, Score
    /// Winner has a Score of 0
    /// </summary>
    [BsonElement("scores")]
    public List<PlayerScoreLegacy> Scores = new List<PlayerScoreLegacy>();

    [BsonElement("timestamp")]
    public ulong TimeStamp { set; get; } = 0;
}

public class PlayerScoreLegacy
{
    public PlayerScoreLegacy() {}
    
    [BsonConstructor]
    public PlayerScoreLegacy(long playerId, int score)
    {
        PlayerID = playerId;
        Score = score;
    }

    [BsonElement("player_id")]
    public long PlayerID { set; get; } = 0;

    [BsonElement("score")]
    public int Score { set; get; } = 0;
}