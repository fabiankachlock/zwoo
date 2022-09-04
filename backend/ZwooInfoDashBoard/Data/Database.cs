using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ZwooInfoDashBoard.Data;

public class Database
{
    public Database()
    {
        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new User(p.Id, p.Sid, p.Username, p.Email, p.Password, p.Wins, p.ValidationCode, p.Verified));
        });
        
        BsonClassMap.RegisterClassMap<BetaCode>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new BetaCode(p.Id, p.Code));
        });
        
        BsonClassMap.RegisterClassMap<GameInfo>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new GameInfo(p.Id, p.GameName, p.GameID, p.IsPublic, p.Scores, p.TimeStamp));
        });
        
        BsonClassMap.RegisterClassMap<PlayerScore>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new PlayerScore(p.PlayerID, p.Score));
        });
        
        BsonClassMap.RegisterClassMap<AccountEvent>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new AccountEvent(p.Id, p.EventType, p.PlayerID, p.Success, p.TimeStamp));
        });
        
        
        var client = new MongoClient(Globals.ConnectionString);
        Console.WriteLine($"connected to {Globals.ConnectionString}");

        _database = client.GetDatabase("zwoo");

        if (_database == null)
        {
            Console.WriteLine("cant connect to database");
            Environment.Exit(1);
        }

        Console.WriteLine($"established connection with database");

        _userCollection = _database.GetCollection<User>("users");
        _gameInfoCollection = _database.GetCollection<GameInfo>("game_info");
        _accountEventsCollection = _database.GetCollection<AccountEvent>("account_events");
        _betacodesCollection = _database.GetCollection<BetaCode>("betacodes");

    }

    public IQueryable<GameInfo> GetPlayedGamesAsQueryable() => _gameInfoCollection.AsQueryable();
    public IQueryable<User> GetUsersAsQueryable() => _userCollection.AsQueryable();
    public IQueryable<BetaCode> GetBetaCodes() => _betacodesCollection.AsQueryable();
    public IQueryable<AccountEvent> GetAccountEventsAsQueryable() => _accountEventsCollection.AsQueryable();
    public IQueryable<AccountEvent> GetUserAccountEvents(ulong id) => _accountEventsCollection.AsQueryable().Where(x => x.PlayerID == id);
    public void UpdateUser(User user) => _userCollection.ReplaceOne(x=> x.Id == user.Id, user);
    public User GenerateUser()
    {
        var user = new User();
        user.Id =
            _userCollection.Find(x => true).Sort(new BsonDocument { { "_id", -1 } }).Limit(1).ToList().First().Id + 1;
        _userCollection.InsertOne(user);
        return user;
    }
    public void DeleteUser(User user) => _userCollection.DeleteOne(x => x.Id == user.Id);
    public void InsertBetacode(BetaCode code) => _betacodesCollection.InsertOne(code);
    public User GetUser(ulong id) => _userCollection.Find(x => x.Id == id).First();

    public List<User> GetLeaderboard()
    {
        var players = new List<User>();
        players = _userCollection.Find(Builders<User>.Filter.Eq(u => u.Verified, true)).Sort(new BsonDocument { { "wins", -1 } })
            .Limit(100).ToList();
        for (int i = 0; i <= players.Count - 1; i++)
            players[i].Position = i + 1;
        return players;
    }

    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<GameInfo> _gameInfoCollection;
    private readonly IMongoCollection<AccountEvent> _accountEventsCollection;
    private readonly IMongoCollection<BetaCode> _betacodesCollection;
}