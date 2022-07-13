using System.Text.Json.Serialization;

namespace ZwooBackend.Controllers.DTO;

public class LeaderBoard
{
    [JsonPropertyName("leaderboard")] 
    public List<LeaderBoardPlayer>? TopPlayers { set; get; }
}

public class LeaderBoardPlayer
{
    public LeaderBoardPlayer(string username, uint wins)
    {
        Username = username;
        Wins = wins;
    }

    [JsonPropertyName("username")]
    public string Username { set; get; }
    [JsonPropertyName("wins")]
    public UInt32 Wins { set; get; }
}