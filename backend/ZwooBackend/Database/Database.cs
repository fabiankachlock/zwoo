using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using BackendHelper;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Quartz;
using ZwooBackend.Controllers;
using ZwooBackend.Controllers.DTO;
using static ZwooBackend.Globals;

namespace ZwooBackend.Database;

public class DatabaseCleanupJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Logger.Info("Starting DB Cleanup");
        ZwooDatabase.CleanDatabase();
        Logger.Info("DB Cleanup Finished!");
        return Task.CompletedTask;
    }
}

public class Database
{
    [Serializable]
    private class DatabaseException : Exception
    {
        public DatabaseException() : base() {}
        public DatabaseException(string message) : base(message) {}
    }
    
    public Database()
    {
        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new User(p.Id, p.Sid, p.Username, p.Email, p.Password, p.Wins, p.ValidationCode, p.Verified));
        });

        var client = new MongoClient(ConnectionString);
        Logger.Info($"Established MongoDB Connection to {ConnectionString}");

        _database = client.GetDatabase("zwoo");

        if (_database == null)
        {
            Logger.Error("Couldn't connect to zwoo database");
            Environment.Exit(1);
        }
            
        Logger.Info($"Connected to zwoo Database");

        _collection = _database.GetCollection<User>("users");

        var t = _collection.Find(x => true).Sort(new BsonDocument {{"_id", -1}}).Limit(1).ToList();
        _generator = t.Count != 0 ? new UIDGenerator(t[0].Id) : new UIDGenerator(0);
    }

    public (string, ulong, string, string) CreateUser(string username, string email, string password, string? betaCode)
    {
        var code = StringHelper.GenerateNDigitString(6);
        var id = _generator.GetNextID();
        
        var salt = RandomNumberGenerator.GetBytes(16);
        var pw = Encoding.ASCII.GetBytes(password).Concat(salt).ToArray();
        
        using (var sha = SHA512.Create())
        {
            foreach (int i in Enumerable.Range(0, 10000)) pw = sha.ComputeHash(pw);
        }

        var user = new User
        {
            Id = id,
            Sid = "",
            Username = username,
            Email = email,
            Password = $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}",
            Wins = 0,
            ValidationCode = code.Trim().Normalize(),
            Verified = false,
            BetaCode = betaCode
        };

        _collection.InsertOne(user);
        
        return (username, id, code, email);
    }

    public bool EntryExists(string field, string value)
    {
        var res = _collection.Find(new BsonDocument { { field, value } }).ToList();
        return res.Count != 0;
    }

    public bool CheckBetaCode(string? betaCode)
    {
        if (betaCode == null) return false;
        Logger.Info(betaCode);
        var coll = _database.GetCollection<BsonDocument>("betacodes");
        return coll.Find(new BsonDocument { { "code", betaCode! } }).ToList().Count != 0;
    }

    private bool RemoveBetaCode(string? betaCode)
    {
        if (betaCode == null) return false;
        var coll = _database.GetCollection<BsonDocument>("betacodes");
        return coll.DeleteOne(new BsonDocument { { "code", betaCode! } }).DeletedCount != 0;
    }

    public bool VerifyUser(ulong id, string code)
    {
        var user = _collection.Find(x => x.Id == id).FirstOrDefault();
        if (!RemoveBetaCode(user.BetaCode))
            return false;
        var filter = Builders<User>.Filter.Eq(u => u.Id, id) & Builders<User>.Filter.Eq(u => u.ValidationCode, code);
        var update = Builders<User>.Update.Set(u => u.Verified, true);
        return _collection.UpdateOne(filter, update).ModifiedCount != 0;
    }

    public bool LoginUser(string email, string password, out string sid, out UInt64 id, out ErrorCodes.Errors error)
    {
        error = ErrorCodes.Errors.NONE;
        sid = "";
        id = 0;
        var u = _collection.Find(Builders<User>.Filter.Eq(u => u.Email, email)).ToList();
        if (u.Count == 0)
        {
            error = ErrorCodes.Errors.USER_NOT_FOUND;
            return false;
        }
        var user = u[0];
        
        var salt = Convert.FromBase64String(user.Password.Split(':')[1]);
        var pw = Encoding.ASCII.GetBytes(password).Concat(salt).ToArray();
        
        using (var sha = SHA512.Create())
        {
            pw = Enumerable.Range(0, 10000).Aggregate(pw, (current, i) => sha.ComputeHash(current));
        }

        if (Convert.ToBase64String(pw) == user.Password.Split(':')[2] && user.Verified)
        {
            id = user.Id;
            sid = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.Sid, sid);
            return _collection.UpdateOne(filter, update).ModifiedCount != 0;
        }
        error = ErrorCodes.Errors.PASSWORD_NOT_MATCHING;
        return false;
    }

    public bool GetUser(string cookie, out User user)
    {
        user = new User();
        var cookieData = cookie.Split(",");
        var u = _collection.Find(Builders<User>.Filter.Eq<ulong>(u=> u.Id, Convert.ToUInt64(cookieData[0]))).ToList();
        if (u.Count == 0)
            return false;
        user = u[0];
        if (user.Sid == cookieData[1])
            return true;
        return false;
    }

    public void LogoutUser(User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id , user.Id);
        var update = Builders<User>.Update.Set(u => u.Sid, "");
        _collection.UpdateOne(filter, update);
    }

    public bool DeleteUser(User user, string password)
    {
        var salt = Convert.FromBase64String(user.Password.Split(':')[1]);
        var pw = Encoding.ASCII.GetBytes(password).Concat(salt).ToArray();
        
        using (var sha = SHA512.Create())
        {
            pw = Enumerable.Range(0, 10000).Aggregate(pw, (current, i) => sha.ComputeHash(current));
        }

        if (Convert.ToBase64String(pw) != user.Password.Split(':')[2] || !user.Verified) return false;
        _collection.DeleteOne(Builders<User>.Filter.Eq(u => u.Id, user.Id));
        return true;
    }

    public LeaderBoard GetLeaderBoard()
    {
        var leaderboard = new LeaderBoard
        {
            TopPlayers = new List<LeaderBoardPlayer>()
        };

        var players = _collection.Find(Builders<User>.Filter.Eq(u => u.Verified, true)).Sort(new BsonDocument { { "wins", -1 } })
            .Limit(100).ToList();

        if (players == null) return leaderboard;
        foreach (var player in players)
        {
            leaderboard.TopPlayers.Add(new LeaderBoardPlayer(player.Username, player.Wins));
        }

        return leaderboard;
    }
    
    public long GetPosition( User user )
    {
        return _collection.Aggregate().Match(Builders<User>.Filter.Gte(u => u.Wins, user.Wins)).ToList().Count;
    }

    public void CleanDatabase()
    {
        Logger.Info($"Deleted {_collection.DeleteMany(x => x.Verified == false).DeletedCount} User(s).");
    }

    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<User> _collection;

    private readonly UIDGenerator _generator;
}