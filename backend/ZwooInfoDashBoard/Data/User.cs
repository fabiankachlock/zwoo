using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooInfoDashBoard.Data;

public class User
{
    public User() {}
    
    [BsonConstructor]
    public User(ulong id, string sid, string username, string email, string password, uint wins, string validationCode, bool verified)
    {
        Id = id;
        Sid = sid;
        Username = username;
        Email = email;
        Password = password;
        Wins = wins;
        ValidationCode = validationCode;
        Verified = verified;
    }

    [BsonElement("_id")]
    public UInt64 Id { set; get; } = 0;
    
    [BsonElement("sid")]
    [BsonIgnoreIfDefault]
    public string Sid { set; get; } = "";
    
    [BsonElement("username")]
    public string Username { set; get; } = "";
    
    [BsonElement("email")]
    public string Email { set; get; } = "";
    
    [BsonElement("password")]
    public string Password { set; get; } = "";
    
    [BsonElement("wins")]
    public UInt32 Wins { set; get; } = 0;
    
    [BsonElement("validation_code")]
    [BsonIgnoreIfDefault]
    public string ValidationCode { set; get; } = "";
    
    [BsonElement("verified")]
    public bool Verified { set; get; } = false;
    
    [BsonElement("beta_code")]
    [BsonIgnoreIfDefault]
    public string? BetaCode { set; get; }

    [BsonIgnore]
    public int Position { set; get; } = -1;
}