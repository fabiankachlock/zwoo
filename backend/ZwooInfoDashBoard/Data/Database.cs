using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ZwooDatabaseClasses;

namespace ZwooInfoDashBoard.Data;

public class Database
{
    public Database()
    {
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
        _changelogCollection = _database.GetCollection<Changelog>("changelogs");
        

    }
    public IQueryable<GameInfo> GetPlayedGamesAsQueryable() => _gameInfoCollection.AsQueryable();
    public IQueryable<User> GetUsersAsQueryable() => _userCollection.AsQueryable();
    public IQueryable<BetaCode> GetBetaCodesAsQueryable() => _betacodesCollection.AsQueryable();
    public IQueryable<Changelog> GetChangelogs() => _changelogCollection.AsQueryable();
    public IQueryable<AccountEvent> GetAccountEventsAsQueryable() => _accountEventsCollection.AsQueryable();
    public IQueryable<AccountEvent> GetUserAccountEvents(ulong id) => _accountEventsCollection.AsQueryable().Where(x => x.PlayerID == id);
    public void UpdateUser(User user) => _userCollection.ReplaceOne(x=> x.Id == user.Id, user);
    public void UpdateChangelog(Changelog changelog) => _changelogCollection.ReplaceOne(x=> x.Id == changelog.Id, changelog);
    public User GetUser(ulong id) => _userCollection.Find(x => x.Id == id).First();
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
    public Changelog InsertVersion(Changelog changelog)
    {
        if (_changelogCollection.AsQueryable().FirstOrDefault(x => x.Version == changelog.Version) == null )
            _changelogCollection.InsertOne(changelog);
        return _changelogCollection.AsQueryable().First(x => x.Version == changelog.Version);
    }

    public void InsertUser(User user) => _userCollection.InsertOne(user);

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
    private readonly IMongoCollection<Changelog> _changelogCollection;
}