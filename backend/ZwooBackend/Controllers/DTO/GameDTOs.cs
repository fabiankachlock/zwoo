using System.Text.Json.Serialization;
using ZwooBackend.ZRP;

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

public class JoinGame
{
    [JsonPropertyName("name")]
    public string? Name { set; get; }

    [JsonPropertyName("password")]
    public string Password { set; get; }

    [JsonPropertyName("use_password")]
    public Boolean? UsePassword { set; get; }

    [JsonPropertyName("opcode")]
    public ZRPRole Opcode { set; get; }

    [JsonPropertyName("guid")]
    public long? GameId { set; get; }

    public JoinGame(string? name, string password, bool? usePassword, ZRPRole opcode, long? gameId)
    {
        Name = name;
        Password = password;
        UsePassword = usePassword;
        Opcode = opcode;
        GameId = gameId;
    }
}

public class JoinGameResponse
{

    [JsonPropertyName("guid")]
    public long GameId { set; get; }

    public JoinGameResponse(long gameId)
    {
        GameId = gameId;
    }
}

public class GameMetaResponse
{
    [JsonPropertyName("id")]
    public long Id { set; get; }

    [JsonPropertyName("name")]
    public string Name { set; get; }

    [JsonPropertyName("isPublic")]
    public bool IsPublic { set; get; }

    [JsonPropertyName("playerCount")]
    public int PlayerCount { set; get; }

    public GameMetaResponse(long id, string name, bool isPublic, int playerCount)
    {
        Id = id;
        Name = name;
        IsPublic = isPublic;
        PlayerCount = playerCount;
    }
}

public class GamesListResponse
{

    [JsonPropertyName("games")]
    public GameMetaResponse[] Games { set; get; }

    public GamesListResponse(GameMetaResponse[] games)
    {
        Games = games;
    }
}