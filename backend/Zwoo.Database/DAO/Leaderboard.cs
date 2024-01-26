namespace Zwoo.Database.Dao;

public class LeaderBoardDao
{
    public LeaderBoardDao(List<LeaderBoardPlayerDao> players)
    {
        TopPlayers = players;
    }

    public List<LeaderBoardPlayerDao> TopPlayers { set; get; }
}

public class LeaderBoardPlayerDao
{
    public LeaderBoardPlayerDao(string username, uint wins)
    {
        Username = username;
        Wins = wins;
    }

    public string Username { set; get; }

    public uint Wins { set; get; }
}
