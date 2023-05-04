using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabase.Dao;

public class DeletedUser
{
    public DeletedUser() { }

    public DeletedUser(User? user)
    {
        if (user == null)
            return;
        Username = user.Username;
        Email = user.Email;
        Password = user.Password;
        Wins = user.Wins;
        Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    [BsonConstructor]
    public DeletedUser(string username, string email, string password, uint wins, ulong timestamp)
    {
        Username = username;
        Email = email;
        Password = password;
        Wins = wins;
        Timestamp = timestamp;
    }

    [BsonElement("username")]
    public string Username { set; get; } = "";

    [BsonElement("email")]
    public string Email { set; get; } = "";

    [BsonElement("password")]
    public string Password { set; get; } = "";

    [BsonElement("wins")]
    public UInt32 Wins { set; get; } = 0;

    [BsonElement("timestamp")]
    public ulong Timestamp;
}