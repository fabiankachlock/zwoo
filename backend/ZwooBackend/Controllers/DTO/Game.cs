using System.Text.Json.Serialization;
using ZwooGameLogic.ZRP;
using ZwooDatabase.Dao;

namespace ZwooBackend.Controllers.DTO;

public static class LeaderboardPlayerDaoExtensions
{
    public static LeaderBoardPlayer ToDTO(this LeaderBoardPlayerDao dao)
    {
        return new LeaderBoardPlayer()
        {
            Username = dao.Username,
            Wins = dao.Wins,
        };
    }
}

public static class LeaderboardDaoExtensions
{
    public static LeaderBoard ToDTO(this LeaderBoardDao dao)
    {
        return new LeaderBoard()
        {
            TopPlayers = dao.TopPlayers.Select(playerDao => playerDao.ToDTO()).ToList()
        };
    }
}

public class LeaderBoard
{
    public LeaderBoard() { }

    [JsonPropertyName("leaderboard")]
    public List<LeaderBoardPlayer> TopPlayers { set; get; } = new();
}

public class LeaderBoardPlayer
{
    public LeaderBoardPlayer() { }

    [JsonPropertyName("username")]
    public string Username { set; get; } = "";

    [JsonPropertyName("wins")]
    public uint Wins { set; get; } = 0;
}

public class LeaderBoardPosition
{
    public LeaderBoardPosition() { }

    [JsonPropertyName("position")]
    public ulong Position { set; get; } = 0;
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

    [JsonPropertyName("isRunning")]
    public bool IsRunning { set; get; }

    [JsonPropertyName("role")]
    public ZRPRole Role { set; get; }


    public JoinGameResponse(long gameId, bool isRunning, ZRPRole role)
    {
        GameId = gameId;
        IsRunning = isRunning;
        Role = role;
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