using MongoDB.Driver;
using MongoDB.Bson;
using Zwoo.Database.Dao;
using log4net;

namespace Zwoo.Database;

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
    public void SaveGame(GameInfoDao data, AuditOptions? auditOptions = null);
}

public class GameInfoService : IGameInfoService
{

    private readonly IDatabase _db;
    private readonly IAuditTrailService _audits;
    private readonly ILog _logger;

    public GameInfoService(IDatabase db, IAuditTrailService audits, ILog? logger = null)
    {
        _db = db;
        _audits = audits;
        _logger = logger ?? LogManager.GetLogger("GAmeInfoService");
    }


    public void SaveGame(GameInfoDao data, AuditOptions? auditOptions = null)
    {
        _logger.Info($"saving game {data.GameId}");
        _db.GamesInfo.InsertOne(data);
        foreach (var player in data.Scores)
        {
            // TODO: avoid querying each user individually
            var user = _db.Users.AsQueryable().FirstOrDefault(user => user.Username == player.PlayerUsername);
            if (user != null)
            {
                // TODO: would be great to have the active session id here
                _audits.Protocol(_audits.GetAuditId(user), AuditOptions.CreateEvent(auditOptions, new AuditEventDao()
                {
                    Actor = AuditActor.System,
                    Message = "created game info object",
                    NewValue = data,
                }));
            }
        }
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