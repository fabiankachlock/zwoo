using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabase.Dao;

public class DeletedUserDao
{
    public DeletedUserDao() { }

    public DeletedUserDao(UserDao? user)
    {
        if (user == null)
            return;
        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
        Password = user.Password;
        Wins = user.Wins;
        Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    [BsonConstructor]
    public DeletedUserDao(ulong id, string username, string email, string password, uint wins, ulong timestamp)
    {
        Id = id;
        Username = username;
        Email = email;
        Password = password;
        Wins = wins;
        Timestamp = timestamp;
    }

    [BsonElement("_id")]
    public UInt64 Id { set; get; } = 0;

    [BsonElement("username")]
    public string Username { set; get; } = "";

    [BsonElement("email")]
    public string Email { set; get; } = "";

    [BsonElement("password")]
    public string Password { set; get; } = "";

    [BsonElement("wins")]
    public UInt32 Wins { set; get; } = 0;

    /// <summary>
    /// time in unix seconds
    /// </summary>
    [BsonElement("timestamp")]
    public ulong Timestamp;
}