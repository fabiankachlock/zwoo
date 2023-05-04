using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabase.Dao;

[RuntimeVersion("1.0.0-beta.7")]
[StartUpVersion("1.0.0-beta.7")]
[CollectionLocation("users")]
public class User : IDocument
{
    public User() { }

    [BsonConstructor]
    public User(ulong id, List<string> sid, string username, string email, string password, uint wins, string settings, string validationCode, bool verified)
    {
        Id = id;
        Sid = sid;
        Username = username;
        Email = email;
        Password = password;
        Wins = wins;
        Settings = settings;
        ValidationCode = validationCode;
        Verified = verified;
    }

    [BsonElement("_id")]
    public UInt64 Id { set; get; }

    [BsonElement("sid")]
    [BsonIgnoreIfDefault]
    public List<string> Sid { set; get; } = new();

    [BsonElement("username")]
    public string Username { set; get; } = "";

    [BsonElement("email")]
    public string Email { set; get; } = "";

    [BsonElement("password")]
    public string Password { set; get; } = "";

    [BsonElement("wins")]
    public UInt32 Wins { set; get; }

    [BsonElement("settings")]
    [BsonIgnoreIfDefault]
    public string Settings { set; get; } = "";

    [BsonElement("validation_code")]
    [BsonIgnoreIfDefault]
    public string ValidationCode { set; get; } = "";

    [BsonElement("verified")]
    public bool Verified { set; get; }

    [BsonElement("beta_code")]
    [BsonIgnoreIfDefault]
    public string? BetaCode { set; get; }

    [BsonElement("password_reset_code")]
    [BsonIgnoreIfDefault]
    public string? PasswordResetCode { set; get; }

    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}