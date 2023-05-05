using MongoDB.Driver;
using MongoDB.Bson;
using ZwooDatabase.Dao;

namespace ZwooDatabase;

/// <summary>
/// a service for executing game info related database operations
/// </summary>
public interface IGameInfoService
{
    /// <summary>
    /// return the current leader board
    /// </summary>
    public LeaderBoardDao GetLeaderBoard();

    /// <summary>
    /// return the current position for a user
    /// </summary>
    /// <param name="user">the user</param>
    public ulong GetPosition(UserDao user);

    /// <summary>
    /// save a game
    /// </summary>
    /// <param name="data">the game to</param>
    public void SaveGame(GameInfoDao data);
}

public class GameInfoService : IGameInfoService
{

    private IDatabase _db;

    public GameInfoService(IDatabase db)
    {
        _db = db;
    }


    public void SaveGame(GameInfoDao data)
    {
        _db.GamesInfo.InsertOne(data);
    }

    public ulong GetPosition(UserDao user)
    {
        return (ulong)_db.Users.Aggregate().Match(Builders<UserDao>.Filter.Gte(u => u.Wins, user.Wins)).ToList().Count;
    }

    public LeaderBoardDao GetLeaderBoard()
    {
        var players = _db.Users.Find(x => x.Verified)
            .Sort(new BsonDocument { { "wins", -1 } })
            .Limit(100)
            .ToList()
            .Select(user => new LeaderBoardPlayerDao(user.Username, user.Wins));

        return new LeaderBoardDao(players.ToList());
    }
}