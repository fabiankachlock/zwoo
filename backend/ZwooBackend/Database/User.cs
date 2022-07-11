using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooBackend.Database;

public class User
{
    public User() {}
    
    [BsonConstructor]
    public User(UInt64 id, string sid, string username, string email, string password, uint wins, string validationCode, bool verified)
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

    [BsonRepresentation(BsonType.Int64)]
    [BsonElement("_id")]
    public UInt64 Id { set; get; } = 0;
    
    [BsonRepresentation(BsonType.String)]
    [BsonElement("sid")]
    [BsonIgnoreIfDefault]
    public string Sid { set; get; } = "";
    
    [BsonRepresentation(BsonType.String)]
    [BsonElement("username")]
    public string Username { set; get; } = "";
    
    [BsonRepresentation(BsonType.String)]
    [BsonElement("email")]
    public string Email { set; get; } = "";
    
    [BsonRepresentation(BsonType.String)]
    [BsonElement("password")]
    public string Password { set; get; } = "";
    
    [BsonRepresentation(BsonType.Int32)]
    [BsonElement("wins")]
    public UInt32 Wins { set; get; } = 0;
    
    [BsonRepresentation(BsonType.String)]
    [BsonElement("validation_code")]
    [BsonIgnoreIfDefault]
    public string ValidationCode { set; get; } = "";
    
    [BsonRepresentation(BsonType.Boolean)]
    [BsonElement("verified")]
    public bool Verified { set; get; } = false;
}