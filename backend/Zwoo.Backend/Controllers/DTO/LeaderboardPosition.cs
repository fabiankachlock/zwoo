using System.Text.Json.Serialization;
using Zwoo.Database.Dao;

namespace Zwoo.Backend.Controllers.DTO;


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

    public string Username { set; get; } = string.Empty;

    public uint Wins { set; get; }
}