using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseClasses;

public class User
{
    public User() {}
    
    [BsonConstructor]
    public User(ulong id, List<string> sid, string username, string email, string password, uint wins, string validationCode, bool verified)
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

    [JsonIgnore]
    [BsonElement("_id")]
    public UInt64 Id { set; get; } = 0;
    
    [BsonElement("sid")]
    [JsonIgnore]
    [BsonIgnoreIfDefault]
    public List<string> Sid { set; get; } = new();
    
    [JsonPropertyName("username")]
    [BsonElement("username")]
    public string Username { set; get; } = "";
    
    [JsonPropertyName("email")]
    [BsonElement("email")]
    public string Email { set; get; } = "";
    
    [JsonIgnore]
    [BsonElement("password")]
    public string Password { set; get; } = "";
    
    [JsonPropertyName("wins")]
    [BsonElement("wins")]
    public UInt32 Wins { set; get; } = 0;
    
    [BsonElement("validation_code")]
    [JsonIgnore]
    [BsonIgnoreIfDefault]
    public string ValidationCode { set; get; } = "";
    
    [JsonIgnore]
    [BsonElement("verified")]
    public bool Verified { set; get; } = false;
    
    [BsonElement("beta_code")]
    [JsonIgnore]
    [BsonIgnoreIfDefault]
    public string? BetaCode { set; get; }
    
    [BsonElement("password_reset_code")]
    [JsonIgnore]
    [BsonIgnoreIfDefault]
    public string? PasswordResetCode { set; get; }
    
    [BsonIgnore] public int Position { set; get; } = -1;
}