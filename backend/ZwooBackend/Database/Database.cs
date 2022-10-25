using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using BackendHelper;
using MongoDB.Bson.Serialization;
using Quartz;
using ZwooBackend.Controllers;
using ZwooBackend.Controllers.DTO;
using ZwooDatabaseClasses;
using ZwooGameLogic.Game;
using static ZwooBackend.Globals;

namespace ZwooBackend.Database;

public class DatabaseCleanupJob : IJob
{
    // Periodic Database Cleanup job
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
        var client = new MongoClient(ConnectionString);
        DatabaseLogger.Info($"connected to {ConnectionString}");

        _database = client.GetDatabase("zwoo");

        if (_database == null)
        {
            DatabaseLogger.Error("cant connect to database");
            Environment.Exit(1);
        }

        DatabaseLogger.Info($"established connection with database");

        _betacodesCollection = _database.GetCollection<BetaCode>("betacodes");
        _userCollection = _database.GetCollection<User>("users");
        _gameInfoCollection = _database.GetCollection<GameInfo>("game_info");
        _accountEventCollection = _database.GetCollection<AccountEvent>("account_events");
        _changelogCollection = _database.GetCollection<Changelog>("changelogs");
        
        if (_changelogCollection.AsQueryable().FirstOrDefault(c => c.Version == Globals.Version) == null)
            _changelogCollection.InsertOne(new Changelog(Globals.Version, ""));
    }

    /// <summary>
    /// Hash Password, Generate verification code and Creates user in Database
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="email">Email</param>
    /// <param name="password">Password to Hash</param>
    /// <param name="betaCode">Betacode if needed</param>
    /// <returns>Username, player-id, verification code, email-address</returns>
    public (string, ulong, string, string) CreateUser(string username, string email, string password, string? betaCode)
    {
        DatabaseLogger.Debug($"[User] creating {username}");
        var code = StringHelper.GenerateNDigitString(6);
        var id = _userCollection.AsQueryable().Max(x => x.Id) + 1;

        var salt = RandomNumberGenerator.GetBytes(16);
        var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(password).Concat(salt).ToArray());

        var user = new User
        {
            Id = id,
            Sid = new(),
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
    
    public bool UsernameExists(string username) => _userCollection.AsQueryable().FirstOrDefault(x => x.Username == username) != null;
    
    public bool EmailExists(string email) => _userCollection.AsQueryable().FirstOrDefault(x => x.Email == email) != null;
    
    public bool CheckBetaCode(string? betaCode)
    {
        if (betaCode == null) return false;
        DatabaseLogger.Debug($"[BetaCode] checking {betaCode}");
        return _betacodesCollection.AsQueryable().FirstOrDefault(x => x.Code == betaCode) != null;
    }
    
    private bool RemoveBetaCode(string? betaCode)
    {
        if (betaCode == null) return false;
        DatabaseLogger.Debug($"[BetaCode] removing {betaCode}");
        return _betacodesCollection.DeleteOne(x => betaCode == x.Code).DeletedCount != 0;
    }
    
    public bool VerifyUser(ulong id, string code)
    {
        DatabaseLogger.Debug($"[User] verifying {id}");
        var user = _userCollection.AsQueryable().FirstOrDefault(x => x.Id == id && x.ValidationCode == code);
        
        if (user != null) return false;
        if (Globals.IsBeta && !RemoveBetaCode(user.BetaCode))
            return false;
        
        var res = _userCollection.UpdateOne(x => x.Id == id && x.BetaCode == code, Builders<User>.Update.Set(u => u.Verified, true)).ModifiedCount != 0;
        VerifyAttempt(id, res);
        return res;
    }

    /// <summary>
    /// Logs a user in
    /// </summary>
    /// <param name="email">user-email</param>
    /// <param name="password">entered password</param>
    /// <param name="sid">session-id for Cookie</param>
    /// <param name="id">player-id for Cookie</param>
    /// <param name="error">login-error</param>
    /// <returns>login successful</returns>
    public bool LoginUser(string email, string password, out string sid, out UInt64 id, out ErrorCodes.Errors error)
    {
        DatabaseLogger.Debug($"[User] login {email}");
        error = ErrorCodes.Errors.NONE;
        sid = "";
        id = 0;
        var user = _userCollection.AsQueryable().FirstOrDefault(x => x.Email == email);
        if (user == null)
        {
            error = ErrorCodes.Errors.USER_NOT_FOUND;
            return false;
        }

        if (StringHelper.CheckPassword(password, user.Password) && user.Verified)
        {
            id = user.Id;
            sid = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var res = _userCollection.UpdateOne(x => x.Id == user.Id, Builders<User>.Update.Push(u => u.Sid, sid)).ModifiedCount != 0;
            LoginAttempt(user.Id, res);
            return res;
        }
        error = user.Verified ? ErrorCodes.Errors.PASSWORD_NOT_MATCHING : ErrorCodes.Errors.USER_NOT_VERIFIED;
        LoginAttempt(user.Id, false);
        return false;
    }

    /// <summary>
    /// Gets user from Cookie
    /// </summary>
    /// <param name="cookie">cookie from user</param>
    /// <param name="user">user</param>
    /// <returns>user found</returns>
    public bool GetUser(string cookie, out User user, out string sid)
    {
        user = new User();
        sid = "";
        var cookieData = cookie.Split(",");
        user = _userCollection.AsQueryable().FirstOrDefault(x => x.Id == Convert.ToUInt64(cookieData[0]));
        if (user == null)
            return false;
        if (user.Sid.Contains(cookieData[1]))
        {
            sid = cookieData[1];
            return true;
        }
        return false;
    }
    
    public void LogoutUser(User user, string sid)
    {
        DatabaseLogger.Debug($"[User] logout {user.Email}");
        LogoutAttempt(user.Id, _userCollection.UpdateOne(u => u.Id == user.Id, Builders<User>.Update.Pull(u => u.Sid, sid)).ModifiedCount != 0);
    }

    /// <summary>
    /// Deletes user after password matches
    /// </summary>
    /// <param name="user">user to delete</param>
    /// <param name="password">password of user</param>
    /// <returns></returns>
    public bool DeleteUser(User user, string password)
    {
        DatabaseLogger.Debug($"[User] deleting {user.Email}");

        if (!StringHelper.CheckPassword(password, user.Password) || !user.Verified)
        {
            DeleteAttempt(user.Id, false);
            return false;
        }
        _userCollection.DeleteOne(x => x.Id == user.Id);
        DeleteAttempt(user.Id, true, user);
        return true;
    }
    
    public LeaderBoard GetLeaderBoard()
    {
        var leaderboard = new LeaderBoard
        {
            TopPlayers = new List<LeaderBoardPlayer>()
        };

        var players = _userCollection.Find(x => x.Verified).Sort(new BsonDocument { { "wins", -1 } }).Limit(100).ToList();

        if (players == null) return leaderboard;
        foreach (var player in players) leaderboard.TopPlayers.Add(new LeaderBoardPlayer(player.Username, player.Wins));

        return leaderboard;
    }
    
    public uint IncrementWin(ulong puid)
    {
        DatabaseLogger.Info($"Incrementing win for user {puid}");
        if (_userCollection.UpdateOne(x => x.Id == puid, Builders<User>.Update.Inc(u => u.Wins, (uint)1)).ModifiedCount != 0)
            return _userCollection.AsQueryable().First(u => u.Id == puid).Wins;
        return 0;
    }
    
    public long GetPosition(User user) => _userCollection.Aggregate().Match(Builders<User>.Filter.Gte(u => u.Wins, user.Wins)).ToList().Count;

    public bool ChangePassword(User user, string oldPassword, string newPassword, string sid)
    {
        if (StringHelper.CheckPassword(oldPassword, user.Password))
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(newPassword).Concat(salt).ToArray());
            _userCollection.UpdateOne(x => x.Id == user.Id, Builders<User>.Update.Set(u => u.Sid, new List<string> { sid }));
            return _userCollection.UpdateOne(x => x.Id == user.Id, Builders<User>.Update.Set(u => u.Password, $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}")).ModifiedCount != 0;
        }
        return false;
    }

    public User RequestChangePassword(string email)
    {
        var user = _userCollection.AsQueryable().FirstOrDefault(x => x.Email == email);
        user.PasswordResetCode = Guid.NewGuid().ToString();
        _userCollection.UpdateOne(x => x.Id == user.Id,
            Builders<User>.Update.Set(u => u.PasswordResetCode, user.PasswordResetCode));
        return user;
    }
    
    public void ResetPassword(string code, string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var pw = StringHelper.HashString(Encoding.ASCII.GetBytes(password).Concat(salt).ToArray());
        _userCollection.UpdateOne(x => x.PasswordResetCode == code,
            Builders<User>.Update.Set(u => u.Password,
                $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}"));
        _userCollection.UpdateOne(x => x.PasswordResetCode == code,
            Builders<User>.Update.Set(u => u.PasswordResetCode, ""));
    }
    
    /// <summary>
    /// Delete unverified users & unused password reset codes & delete expired delete account events
    /// </summary>
    public void CleanDatabase()
    {
        {
            var users = _userCollection.AsQueryable().Where(x => !x.Verified);
            DatabaseLogger.Info($"[CleanUp] deleted {users.Count()} user(s).");
            foreach (var user in users)
                DeleteAttempt(user.Id, _userCollection.DeleteOne(x => x.Id == user.Id).DeletedCount == 1, user);
        }
        {
            var users = _userCollection.AsQueryable().Where(x => !String.IsNullOrEmpty(x.PasswordResetCode));
            DatabaseLogger.Info($"[CleanUp] deleted {users.Count()} password reset codes.");
            foreach (var user in users)
                _userCollection.UpdateOne(x => x.Id == user.Id,
                    Builders<User>.Update.Set(u => u.PasswordResetCode, ""));
        }
        {
            DatabaseLogger.Info($"[CleanUp] deleted {_accountEventCollection.DeleteMany(x => x.EventType == "delete" && x.TimeStamp > (ulong)((DateTimeOffset)(DateTime.Today - TimeSpan.FromDays(6))).ToUnixTimeSeconds()).DeletedCount} delete account events.");
        }
    }
    
    public Changelog? GetChangelog(string version) => _changelogCollection.AsQueryable().FirstOrDefault(c => c.Version == version);
    
    public void SaveGame(Dictionary<long, int> scores, GameMeta meta) => 
        _gameInfoCollection.InsertOne(new GameInfo(meta.Name, meta.Id, meta.IsPublic, scores.Select(x => new PlayerScore(_userCollection.AsQueryable().First(y => y.Id == (ulong)x.Key).Username, x.Value)).ToList(), (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()));
    
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
    
    private void DeleteAttempt(ulong puid, bool success, User? user = null)
    {
        var u = new AccountEvent("delete", puid, success, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds())
        {
            UserData = new DeletedUserData(user)
        };
        _accountEventCollection.InsertOne(u);
    }

    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<GameInfo> _gameInfoCollection;
    private readonly IMongoCollection<AccountEvent> _accountEventCollection;
    private readonly IMongoCollection<Changelog> _changelogCollection;
    private readonly IMongoCollection<BetaCode> _betacodesCollection;
}