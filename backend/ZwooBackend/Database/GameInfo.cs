using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooBackend.Database;

public class GameInfo
{
    public GameInfo() {}
    
    [BsonConstructor]
    public GameInfo(string gameName, long gameId, bool isPublic, List<PlayerScore> scores, ulong timeStamp)
    {
        GameName = gameName;
        GameID = gameId;
        IsPublic = isPublic;
        Scores = scores;
        TimeStamp = timeStamp;
    }
    
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
    public List<PlayerScore> Scores;

    [BsonElement("timestamp")]
    public ulong TimeStamp { set; get; } = 0;
}

public class PlayerScore
{
    public PlayerScore() {}
    
    [BsonConstructor]
    public PlayerScore(long playerId, int score)
    {
        PlayerID = playerId;
        Score = score;
    }

    [BsonElement("player_id")]
    public long PlayerID { set; get; } = 0;

    [BsonElement("score")]
    public int Score { set; get; } = 0;
}