using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using BackendHelper;
using MongoDB.Bson.Serialization;
using Quartz;
using ZwooBackend.Controllers;
using ZwooBackend.Controllers.DTO;
using ZwooGameLogic.Game;
using static ZwooBackend.Globals;

namespace ZwooBackend.Database;

public class DatabaseCleanupJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        DatabaseLogger.Info("[CleanUp] starting db cleanup");
        ZwooDatabase.CleanDatabase();
        DatabaseLogger.Info("[CleanUp] finished db cleanup");
        return Task.CompletedTask;
    }
}

public class Database
{
    [Serializable]
    private class DatabaseException : Exception
    {
        public DatabaseException() : base() { }
        public DatabaseException(string message) : base(message) { }
    }

    public Database()
    {
        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new User(p.Id, p.Sid, p.Username, p.Email, p.Password, p.Wins, p.ValidationCode, p.Verified));
        });
        
        BsonClassMap.RegisterClassMap<GameInfo>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new GameInfo(p.GameName, p.GameID, p.IsPublic, p.Scores, p.TimeStamp));
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
                new AccountEvent(p.EventType, p.PlayerID, p.Success, p.TimeStamp));
        });

        var client = new MongoClient(ConnectionString);
        DatabaseLogger.Info($"connected to {ConnectionString}");

        _database = client.GetDatabase("zwoo");

        if (_database == null)
        {
            DatabaseLogger.Error("cant connect to database");
            Environment.Exit(1);
        }

        DatabaseLogger.Info($"established connection with database");

        _userCollection = _database.GetCollection<User>("users");
        _gameInfoCollection = _database.GetCollection<GameInfo>("game_info");
        _accountEventCollection = _database.GetCollection<AccountEvent>("account_events");
    }

    public (string, ulong, string, string) CreateUser(string username, string email, string password, string? betaCode)
    {
        DatabaseLogger.Debug($"[User] creating {username}");
        var code = StringHelper.GenerateNDigitString(6);
        var id = _userCollection.AsQueryable().Max(x => x.Id) + 1;

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

        _userCollection.InsertOne(user);
        CreateAttempt(id, true);
        return (username, id, code, email);
    }

    public bool EntryExists(string field, string value)
    {
        var res = _userCollection.Find(new BsonDocument { { field, value } }).ToList();
        return res.Count != 0;
    }

    public bool CheckBetaCode(string? betaCode)
    {
        if (betaCode == null) return false;
        DatabaseLogger.Debug($"[BetaCode] checking {betaCode}");
        var coll = _database.GetCollection<BsonDocument>("betacodes");
        return coll.Find(new BsonDocument { { "code", betaCode! } }).ToList().Count != 0;
    }

    private bool RemoveBetaCode(string? betaCode)
    {
        if (betaCode == null) return false;
        DatabaseLogger.Debug($"[BetaCode] removing {betaCode}");
        var coll = _database.GetCollection<BsonDocument>("betacodes");
        return coll.DeleteOne(new BsonDocument { { "code", betaCode! } }).DeletedCount != 0;
    }

    public bool VerifyUser(ulong id, string code)
    {
        DatabaseLogger.Debug($"[User] verifying {id}");
        var user = _userCollection.Find(x => x.Id == id).FirstOrDefault();
        if (Globals.IsBeta && !RemoveBetaCode(user.BetaCode))
            return false;
        var filter = Builders<User>.Filter.Eq(u => u.Id, id) & Builders<User>.Filter.Eq(u => u.ValidationCode, code);
        var update = Builders<User>.Update.Set(u => u.Verified, true);
        var res = _userCollection.UpdateOne(filter, update).ModifiedCount != 0;
        VerifyAttempt(id, res);
        return res;
    }

    public bool LoginUser(string email, string password, out string sid, out UInt64 id, out ErrorCodes.Errors error)
    {
        DatabaseLogger.Debug($"[User] login {email}");
        error = ErrorCodes.Errors.NONE;
        sid = "";
        id = 0;
        var u = _userCollection.Find(Builders<User>.Filter.Eq(u => u.Email, email)).ToList();
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
            var res = _userCollection.UpdateOne(filter, update).ModifiedCount != 0;
            LoginAttempt(id, res);
            return res;
        }
        error = ErrorCodes.Errors.PASSWORD_NOT_MATCHING;
        LoginAttempt(id, false);
        return false;
    }

    public bool GetUser(string cookie, out User user)
    {
        user = new User();
        var cookieData = cookie.Split(",");
        var u = _userCollection.Find(Builders<User>.Filter.Eq<ulong>(u => u.Id, Convert.ToUInt64(cookieData[0]))).ToList();
        if (u.Count == 0)
            return false;
        user = u[0];
        if (user.Sid == cookieData[1])
            return true;
        return false;
    }

    public void LogoutUser(User user)
    {
        DatabaseLogger.Debug($"[User] logout {user.Email}");
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        var update = Builders<User>.Update.Set(u => u.Sid, "");
        LogoutAttempt(user.Id, _userCollection.UpdateOne(filter, update).ModifiedCount != 0);
        
    }

    public bool DeleteUser(User user, string password)
    {
        DatabaseLogger.Debug($"[User] deleting {user.Email}");
        var salt = Convert.FromBase64String(user.Password.Split(':')[1]);
        var pw = Encoding.ASCII.GetBytes(password).Concat(salt).ToArray();

        using (var sha = SHA512.Create())
        {
            pw = Enumerable.Range(0, 10000).Aggregate(pw, (current, i) => sha.ComputeHash(current));
        }

        if (Convert.ToBase64String(pw) != user.Password.Split(':')[2] || !user.Verified)
        {
            DeleteAttempt(user.Id, false);
            return false;
        }
        _userCollection.DeleteOne(Builders<User>.Filter.Eq(u => u.Id, user.Id));
        DeleteAttempt(user.Id, true);
        return true;
    }

    public LeaderBoard GetLeaderBoard()
    {
        var leaderboard = new LeaderBoard
        {
            TopPlayers = new List<LeaderBoardPlayer>()
        };

        var players = _userCollection.Find(Builders<User>.Filter.Eq(u => u.Verified, true)).Sort(new BsonDocument { { "wins", -1 } })
            .Limit(100).ToList();

        if (players == null) return leaderboard;
        foreach (var player in players)
        {
            leaderboard.TopPlayers.Add(new LeaderBoardPlayer(player.Username, player.Wins));
        }

        return leaderboard;
    }

    public uint IncrementWin(ulong puid)
    {
        DatabaseLogger.Info($"Incrementing win for user {puid}");
        if (_userCollection.UpdateOne(x => x.Id == puid, Builders<User>.Update.Inc(u => u.Wins, (uint)1)).ModifiedCount != 0)
            return _userCollection.Find(u => u.Id == puid).FirstOrDefault().Wins;
        return 0;
    }

    public long GetPosition(User user) => _userCollection.Aggregate().Match(Builders<User>.Filter.Gte(u => u.Wins, user.Wins)).ToList().Count;

    public void CleanDatabase() => DatabaseLogger.Info($"[CleanUp] deleted {_userCollection.DeleteMany(x => x.Verified == false).DeletedCount} user(s).");

    // Info Stuff
    
    public void SaveGame(Dictionary<long, int> scores, GameMeta meta) => 
        _gameInfoCollection.InsertOne(new GameInfo(meta.Name, meta.Id, meta.IsPublic, scores.Select(x => new PlayerScore(x.Key, x.Value)).ToList(), (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    
    private void CreateAttempt(ulong puid, bool success) =>
        _accountEventCollection.InsertOne(new AccountEvent("create", puid, success,
            (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    
    private void VerifyAttempt(ulong puid, bool success) =>
        _accountEventCollection.InsertOne(new AccountEvent("verify", puid, success,
            (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    private void LoginAttempt(ulong puid, bool success) =>
        _accountEventCollection.InsertOne(new AccountEvent("login", puid, success,
            (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    
    private void LogoutAttempt(ulong puid, bool success) =>
        _accountEventCollection.InsertOne(new AccountEvent("logout", puid, success,
            (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    
    private void DeleteAttempt(ulong puid, bool success) =>
        _accountEventCollection.InsertOne(new AccountEvent("delete", puid, success,
            (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<GameInfo> _gameInfoCollection;
    private readonly IMongoCollection<AccountEvent> _accountEventCollection;
}