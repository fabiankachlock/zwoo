using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooInfoDashBoard.Data;

public class GameInfo
{
    public GameInfo() {}
    
    [BsonConstructor]
    public GameInfo(ObjectId id, string gameName, long gameId, bool isPublic, List<PlayerScore> scores, ulong timeStamp)
    {
        Id = id;
        GameName = gameName;
        GameId = gameId;
        IsPublic = isPublic;
        Scores = scores;
        TimeStamp = timeStamp;
        Winner = Globals.ZwooDatabase.GetUser((ulong)scores.First(x => x.Score == 0).PlayerId).Username;
    }
    
    [BsonElement("_id")]
    public ObjectId Id { set; get; }
    
    [BsonElement("name")]
    public string GameName { set; get; } = "";
    [BsonElement("game_id")]
    public long GameId { set; get; } = 0;
    [BsonElement("is_public")]
    public bool IsPublic { set; get; } = false;
    /// <summary>
    /// PlayerID, Score
    /// Winner has a Score of 0
    /// </summary>
    [BsonElement("scores")]
    public List<PlayerScore> Scores = null!;

    [BsonElement("timestamp")]
    public ulong TimeStamp { set; get; } = 0;

    [BsonIgnore] public string Winner { set; get; } = "";
}

public class PlayerScore
{
    public PlayerScore() {}
    
    [BsonConstructor]
    public PlayerScore(long playerId, int score)
    {
        PlayerId = playerId;
        Score = score;
    }

    [BsonElement("player_id")]
    public long PlayerId { set; get; } = 0;

    [BsonElement("score")]
    public int Score { set; get; } = 0;
    
    [BsonIgnore]
    public User Player => _player ??= Globals.ZwooDatabase.GetUser((ulong)PlayerId);

    [BsonIgnore]
    private User? _player = null;
}