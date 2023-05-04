using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabase.Dao;

public class PlayerScoreDao
{
    public PlayerScoreDao() { }

    [BsonConstructor]
    public PlayerScoreDao(string playerUsername, int score, bool isBot)
    {
        PlayerUsername = playerUsername;
        Score = score;
        IsBot = isBot;
    }

    [BsonElement("player_username")]
    public string PlayerUsername { set; get; } = "";

    [BsonElement("score")]
    public int Score { set; get; } = 0;

    [BsonElement("is_bot")]
    public bool IsBot { set; get; } = false;
}