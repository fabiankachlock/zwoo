using MongoDB.Bson.Serialization;
using ZwooDatabase.Dao;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using log4net;

namespace ZwooDatabase;

[Serializable]
internal class DatabaseException : Exception
{
    public DatabaseException() { }
    public DatabaseException(string message) : base(message) { }
}

public class Database
{
    public readonly IMongoClient Client;
    public readonly IMongoDatabase MongoDB;
    public readonly IMongoCollection<UserDao> Users;
    public readonly IMongoCollection<GameInfoDao> Games;
    public readonly IMongoCollection<AccountEventDao> AccountEvents;
    public readonly IMongoCollection<ChangelogDao> Changelogs;
    public readonly IMongoCollection<BetaCodeDao> BetaCodes;

    private readonly ILog _logger;

    public Database(string connectionString, string dbName, ILog logger)
    {
        Client = new MongoClient(connectionString);
        _logger = logger;
        _logger.Info($"trying to connect to db");
        MongoDB = Client.GetDatabase(dbName);

        if (MongoDB == null)
        {
            _logger.Error("cant connect to database");
            Environment.Exit(1);
        }

        try
        {
            MongoDB.ListCollectionNames();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            _logger.Fatal("closing! failed to connect to database");
            Environment.Exit(1);
        }

        InitializeClasses();

        _logger.Info($"established connection with database");

        BetaCodes = MongoDB.GetCollection<BetaCodeDao>("betacodes");
        Users = MongoDB.GetCollection<UserDao>("users");
        Games = MongoDB.GetCollection<GameInfoDao>("game_info");
        AccountEvents = MongoDB.GetCollection<AccountEventDao>("account_events");
        Changelogs = MongoDB.GetCollection<ChangelogDao>("changelogs");
    }

    /// <summary>
    /// Delete unverified users & unused password reset codes & delete expired delete account events
    /// </summary>
    public void CleanDatabase()
    {
        {
            var users = Users.AsQueryable().Where(x => !x.Verified);
            _logger.Info($"[CleanUp] deleted {users.Count()} user(s).");
            // foreach (var user in users)
            // DeleteAttempt(user.Id, Users.DeleteOne(x => x.Id == user.Id).DeletedCount == 1, user);
        }
        {
            var users = Users.AsQueryable().Where(x => !String.IsNullOrEmpty(x.PasswordResetCode));
            _logger.Info($"[CleanUp] deleted {users.Count()} password reset codes.");
            foreach (var user in users)
                Users.UpdateOne(x => x.Id == user.Id,
                    Builders<UserDao>.Update.Set(u => u.PasswordResetCode, ""));
        }
        {
            _logger.Info($"[CleanUp] deleted {AccountEvents.DeleteMany(x => x.EventType == "delete" && x.TimeStamp > (ulong)((DateTimeOffset)(DateTime.Today - TimeSpan.FromDays(6))).ToUnixTimeSeconds()).DeletedCount} delete account events.");
        }
    }

    static void InitializeClasses()
    {
        BsonClassMap.RegisterClassMap<UserDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new UserDao(p.Id, p.Sid, p.Username, p.Email, p.Password, p.Wins, p.Settings, p.ValidationCode, p.Verified));
        });

        BsonClassMap.RegisterClassMap<BetaCodeDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new BetaCodeDao(p.Id, p.Code));
        });

        BsonClassMap.RegisterClassMap<ChangelogDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(c =>
                new ChangelogDao(c.Id, c.ChangelogVersion, c.ChangelogText, c.Public, c.Timestamp));
        });

        BsonClassMap.RegisterClassMap<PlayerScoreDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new PlayerScoreDao(p.PlayerUsername, p.Score, p.IsBot));
        });

        BsonClassMap.RegisterClassMap<GameInfoDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new GameInfoDao(p.Id, p.GameName, p.GameId, p.IsPublic, p.Scores, p.TimeStamp));
        });

        BsonClassMap.RegisterClassMap<AccountEventDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new AccountEventDao(p.Id, p.EventType, p.PlayerID, p.Success, p.TimeStamp));
        });
    }

}