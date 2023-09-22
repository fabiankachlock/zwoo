using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabase.Dao;

public class UserSessionDao
{
    [BsonElement("_id")]
    public string Id { get; set; } = "";

    /// <summary>
    /// session expiry date in unix seconds
    /// </summary>
    [BsonElement("expires")]
    public long Expires { get; set; }

    public UserSessionDao() { }

    [BsonConstructor]
    public UserSessionDao(string id, long expires)
    {
        Id = id;
        Expires = expires;
    }
}

[RuntimeVersion("1.0.0-beta.15")]
[StartUpVersion("1.0.0-beta.15")]
[CollectionLocation("users")]
public class UserDao : IDocument
{

    public UserDao() { }

    [BsonConstructor]
    public UserDao(ulong id, List<UserSessionDao> sid, string username, string email, string password, uint wins, string settings, string validationCode, bool verified, bool acceptedTerms)
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
        AcceptedTerms = acceptedTerms;
    }

    [BsonElement("_id")]
    public ulong Id { set; get; }

    [BsonElement("sid")]
    [BsonIgnoreIfDefault]
    public List<UserSessionDao> Sid { set; get; } = new();

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

    [BsonElement("accepted_terms")]
    public bool AcceptedTerms { get; set; }

    [BsonElement("verified_at")]
    [BsonIgnoreIfDefault]
    public long? VerifiedAt { get; set; }

    [BsonElement("accepted_terms_at")]
    [BsonIgnoreIfDefault]
    public long? AcceptedTermsAt { get; set; }

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