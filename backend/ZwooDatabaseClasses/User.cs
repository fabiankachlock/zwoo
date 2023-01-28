using System.Text.Json.Serialization;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseClasses;


[RuntimeVersion("1.0.0-beta.7")]
[StartUpVersion("1.0.0-beta.7")]
[CollectionLocation("users", "zwoo")]
public partial class User : IDocument
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

    [BsonElement("settings")]
    [JsonIgnore]
    [BsonIgnoreIfDefault]
    public string Settings { set; get; } = "";

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

    [JsonIgnore]
    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}