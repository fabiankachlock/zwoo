using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using BackendHelper;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using ZwooBackend.Controllers;
using ZwooBackend.Controllers.DTO;

namespace ZwooBackend.Database;

public class Database
{
    [Serializable]
    class DatabaseException : Exception
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

        _client = new MongoClient(Globals.ConnectionString);
        Globals.Logger.Info($"Established MongoDB Connection to {Globals.ConnectionString}");

        _database = _client.GetDatabase("zwoo");

        if (_database == null)
        {
            Globals.Logger.Error("Couldn't connect to zwoo database");
            Environment.Exit(1);
        }
            
        Globals.Logger.Info($"Connected to zwoo Database");

        _collection = _database.GetCollection<User>("users");

        var t = _collection.Find(x => true).Sort(new BsonDocument {{"_id", -1}}).Limit(1).ToList();
        _generator = t.Count != 0 ? new UIDGenerator(t[0].Id) : new UIDGenerator(0);
    }

    public (string, UInt64, string, string) CreateUser(string username, string email, string password)
    {
        var code = StringHelper.GenerateNDigitString(6);
        var id = _generator.GetNextID();
        
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] pw = Encoding.ASCII.GetBytes(password).Concat(salt).ToArray();
        
        using (var sha = SHA512.Create())
        {
            foreach (int i in Enumerable.Range(0, 10000)) pw = sha.ComputeHash(pw);
        }

        var user = new User();
        user.Id = id;
        user.Sid = "";
        user.Username = username;
        user.Email = email;
        user.Password = $"sha512:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(pw)}";
        user.Wins = 0;
        user.ValidationCode = code.Trim().Normalize();
        user.Verified = false;
        
        _collection.InsertOne(user);
        
        return (username, id, code, email);
    }

    public bool EntryExists(string field, string value)
    {
        var res = _collection.Find(new BsonDocument { { field, value } }).ToList();
        return res.Count != 0;
    }

    public bool UseBetaCode(string? beta_code)
    {
        if (beta_code == null) return false;

        var coll = _database.GetCollection<BsonDocument>("betacodes");
        if (coll.DeleteOne(new BsonDocument { { "code", beta_code! } }).DeletedCount == 0)
            return false;
        return true;
    }

    public bool VerifyUser(UInt64 id, string code)
    {
        var user = _collection.Find(x => x.Id == id);
        var filter = Builders<User>.Filter.Eq(u => u.Id, id) & Builders<User>.Filter.Eq(u => u.ValidationCode, code);
        var update = Builders<User>.Update.Set(u => u.Verified, true);
        if(_collection.UpdateOne(filter, update).ModifiedCount == 0)
            return false;
        return true;
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
        
        byte[] salt = Convert.FromBase64String(user.Password.Split(':')[1]);
        byte[] pw = Encoding.ASCII.GetBytes(password).Concat(salt).ToArray();
        
        using (var sha = SHA512.Create())
        {
            foreach (int i in Enumerable.Range(0, 10000)) pw = sha.ComputeHash(pw);
        }

        if (Convert.ToBase64String(pw) == user.Password.Split(':')[2] && user.Verified)
        {
            id = user.Id;
            sid = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.Sid, sid);
            if(_collection.UpdateOne(filter, update).ModifiedCount == 0)
                return false;
            return true;
        }
        error = ErrorCodes.Errors.PASSWORD_NOT_MATCHING;
        return false;
    }

    public bool GetUser(string cookie, out User user)
    {
        user = null;
        var cookie_data = cookie.Split(",");
        var u = _collection.Find(Builders<User>.Filter.Eq<UInt64>(u=> u.Id, Convert.ToUInt64(cookie_data[0]))).ToList();
        if (u.Count == 0)
            return false;
        user = u[0];
        if (user.Sid == cookie_data[1])
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
        byte[] salt = Convert.FromBase64String(user.Password.Split(':')[1]);
        byte[] pw = Encoding.ASCII.GetBytes(password).Concat(salt).ToArray();
        
        using (var sha = SHA512.Create())
        {
            foreach (int i in Enumerable.Range(0, 10000)) pw = sha.ComputeHash(pw);
        }

        if (Convert.ToBase64String(pw) == user.Password.Split(':')[2] && user.Verified)
        {
            _collection.DeleteOne(Builders<User>.Filter.Eq(u => u.Id, user.Id));
            return true;
        }
        return false;
    }

    public LeaderBoard GetLeaderBoard()
    {
        var leaderboard = new LeaderBoard();
        leaderboard.TopPlayers = new List<LeaderBoardPlayer>();

        var players = _collection.Find(Builders<User>.Filter.Eq(u => u.Verified, true)).Sort(new BsonDocument { { "wins", -1 } })
            .Limit(100).ToList();

        if (players != null)
        {
            foreach (var player in players)
            {
                leaderboard.TopPlayers.Add(new LeaderBoardPlayer(player.Username, player.Wins));
            }
        }

        return leaderboard;
    }
    
    public Int64 GetPosition( User user )
    {
        return _collection.Aggregate().Match(Builders<User>.Filter.Gte(u => u.Wins, user.Wins)).ToList().Count;
    }
    
    private MongoClient _client;
    private IMongoDatabase _database;
    private IMongoCollection<User> _collection;

    private UIDGenerator _generator;
}