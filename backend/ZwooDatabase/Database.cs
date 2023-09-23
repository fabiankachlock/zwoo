using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ZwooDatabase.Dao;
using ZwooDatabase.Legacy;
using MongoDB.Driver;
using log4net;

namespace ZwooDatabase;

public interface IDatabase
{
    /// <summary>
    /// a mongo client instance
    /// </summary>
    public IMongoClient Client { get; }

    /// <summary>
    /// an instance of the currently active mongo database
    /// </summary>
    public IMongoDatabase MongoDB { get; }

    /// <summary>
    /// a reference to the users collection
    /// </summary>
    public IMongoCollection<UserDao> Users { get; }

    /// <summary>
    /// a reference to the games info collection
    /// </summary>
    public IMongoCollection<GameInfoDao> GamesInfo { get; }

    /// <summary>
    /// a reference to the account events collection
    /// </summary>
    public IMongoCollection<AccountEventDao> AccountEvents { get; }

    /// <summary>
    /// a reference to the changelogs  collection
    /// </summary>
    public IMongoCollection<ChangelogDao> Changelogs { get; }

    /// <summary>
    /// a reference to the beta codes collection
    /// </summary>
    public IMongoCollection<BetaCodeDao> BetaCodes { get; }

    /// <summary>
    /// a reference to the audit trails collection
    /// </summary>
    public IMongoCollection<AuditTrailDao> AuditTrails { get; }

    /// <summary>
    /// a reference to the contact requests collection
    /// </summary>
    public IMongoCollection<ContactRequest> ContactRequests { get; }


    /// <summary>
    /// register bson class mappers
    /// </summary>
    public void InitializeClasses();
}

[Serializable]
internal class DatabaseException : Exception
{
    public DatabaseException() { }
    public DatabaseException(string message) : base(message) { }
}


public class Database : IDatabase
{
    public IMongoClient Client { get; }
    public IMongoDatabase MongoDB { get; }
    public IMongoCollection<UserDao> Users { get; }
    public IMongoCollection<GameInfoDao> GamesInfo { get; }
    public IMongoCollection<AccountEventDao> AccountEvents { get; }
    public IMongoCollection<ChangelogDao> Changelogs { get; }
    public IMongoCollection<BetaCodeDao> BetaCodes { get; }
    public IMongoCollection<AuditTrailDao> AuditTrails { get; }
    public IMongoCollection<ContactRequest> ContactRequests { get; }

    private readonly ILog _logger;

    public Database(string connectionString, string dbName, ILog? logger = null)
    {
        Client = new MongoClient(connectionString);
        _logger = logger ?? LogManager.GetLogger("Database");
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
            _logger.Fatal("failed to connect to database");
            Environment.Exit(1);
        }

        InitializeClasses();
        _logger.Info($"established connection with database");

        BetaCodes = MongoDB.GetCollection<BetaCodeDao>("betacodes");
        Users = MongoDB.GetCollection<UserDao>("users");
        GamesInfo = MongoDB.GetCollection<GameInfoDao>("game_info");
        AccountEvents = MongoDB.GetCollection<AccountEventDao>("account_events");
        Changelogs = MongoDB.GetCollection<ChangelogDao>("changelogs");
        AuditTrails = MongoDB.GetCollection<AuditTrailDao>("audit_trails");
        ContactRequests = MongoDB.GetCollection<ContactRequest>("contact_request");
    }

    public void InitializeClasses()
    {
        BsonClassMap.RegisterClassMap<UserDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new UserDao(p.Id, p.Sid, p.Username, p.Email, p.Password, p.Wins, p.Settings, p.ValidationCode, p.Verified, p.AcceptedTerms));
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

        BsonClassMap.RegisterClassMap<AuditTrailDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new AuditTrailDao(p.Id, p.Events));
        });

        BsonClassMap.RegisterClassMap<ContactRequest>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new ContactRequest(p.Id, p.Timestamp, p.Name, p.Email, p.Message, p.CaptchaScore, p.Origin));
        });

        BsonClassMap.RegisterClassMap<Beta11UserDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new Beta11UserDao(p.Id, p.Sid, p.Username, p.Email, p.Password, p.Wins, p.Settings, p.ValidationCode, p.Verified));
        });

        BsonClassMap.RegisterClassMap<Beta12UserDao>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new Beta12UserDao(p.Id, p.Sid, p.Username, p.Email, p.Password, p.Wins, p.Settings, p.ValidationCode, p.Verified, p.AcceptedTerms));
        });

        // setup object serializer for AuditTrailEventDaos `object` properties
        var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName?.StartsWith("ZwooDatabase") != false);
        BsonSerializer.RegisterSerializer(objectSerializer);
    }

}