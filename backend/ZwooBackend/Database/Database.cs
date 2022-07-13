using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using BackendHelper;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

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

        _client = new MongoClient(Environment.GetEnvironmentVariable("ZWOO_DATABASE_CONNECTION_STRING"));
        Globals.Logger.Info($"Established MongoDB Connection to {Environment.GetEnvironmentVariable("ZWOO_DATABASE_CONNECTION_STRING")}");

        _database = _client.GetDatabase("zwoo");

        if (_database == null)
            throw new DatabaseException(message: "couldn't connect to zwoo database!");
            
        Globals.Logger.Info($"Connected to zwoo Database");

        _collection = _database.GetCollection<BsonDocument>("users");

        var t = _collection.Find(x => true).Sort(new BsonDocument {{"_id", -1}}).Limit(1).ToList();
        _generator = t.Count != 0 ? new UIDGenerator(BsonSerializer.Deserialize<User>(t[0].ToBson()).Id) : new UIDGenerator(0);
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
        
        _collection.InsertOne(user.ToBsonDocument());
        
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
        var filter = Builders<BsonDocument>.Filter.Eq("_id", (Int64)id) & Builders<BsonDocument>.Filter.Eq("validation_code", code);
        var update = Builders<BsonDocument>.Update.Set("verified", true);
        if(_collection.UpdateOne(filter, update).ModifiedCount == 0)
            return false;
        return true;
    }

    public bool LoginUser(string email, string password, out string sid, out UInt64 id)
    {
        sid = "";
        id = 0;
        var u = _collection.Find(Builders<BsonDocument>.Filter.Eq("email", email)).ToList();
        if (u.Count == 0)
            return false;
        var user = BsonSerializer.Deserialize<User>(u[0].ToBson());
        
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
            var filter = Builders<BsonDocument>.Filter.Eq("email", email);
            var update = Builders<BsonDocument>.Update.Set("sid", sid);
            if(_collection.UpdateOne(filter, update).ModifiedCount == 0)
                return false;
            return true;
        }
        return false;
    }

    public bool GetUser(string cookie, out User user)
    {
        user = null;
        var cookie_data = cookie.Split(",");
        
        var u = _collection.Find(Builders<BsonDocument>.Filter.Eq("_id", Convert.ToInt64(cookie_data[0]))).ToList();
        if (u.Count == 0)
            return false; 
        user = BsonSerializer.Deserialize<User>(u[0].ToBson());
        if (user.Sid == cookie_data[1])
            return true;
        return false;
    }

    public void LogoutUser(User user)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", user.Id);
        var update = Builders<BsonDocument>.Update.Set("sid", "");
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
            _collection.DeleteOne(Builders<BsonDocument>.Filter.Eq("_id", user.Id));
            return true;
        }
        return false;
    }
    
    private MongoClient _client;
    private IMongoDatabase _database;
    private IMongoCollection<BsonDocument> _collection;

    private UIDGenerator _generator;
}