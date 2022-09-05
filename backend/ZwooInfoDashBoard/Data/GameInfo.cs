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
        GameID = gameId;
        IsPublic = isPublic;
        Scores = scores;
        TimeStamp = timeStamp;
        Winner = Globals.ZwooDatabase.GetUser((ulong)scores.First(x => x.Score == 0).PlayerID).Username;
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
    public List<PlayerScore> Scores;

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
        PlayerID = playerId;
        Score = score;
    }

    [BsonElement("player_id")]
    public long PlayerID { set; get; } = 0;

    [BsonElement("score")]
    public int Score { set; get; } = 0;

    public User Player
    {
        get
        {
            if (_player == null)
                _player = Globals.ZwooDatabase.GetUser((ulong)PlayerID);
            return _player;
        }
    }

    private User _player = null;
}