using System.Text.Json.Serialization;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ZwooDatabaseClasses;


[RuntimeVersion("1.0.0-beta.11")]
[StartUpVersion("1.0.0-beta.11")]
[CollectionLocation("contact_request")]
public class ContactRequest : IDocument
{
    public ContactRequest() { }

    [BsonConstructor]
    public ContactRequest(ObjectId id, long timestamp, string name, string email, string message, bool acceptedTerms, long acceptedTermsAt, double captchaScore, string origin)
    {
        Id = id;
        Timestamp = timestamp;
        Name = name;
        Email = email;
        Message = message;
        AcceptedTerms = acceptedTerms;
        AcceptedTermsAt = acceptedTermsAt;
        CaptchaScore = captchaScore;
        Origin = origin;
    }

    [JsonIgnore]
    [BsonElement("_id")]
    public ObjectId Id { set; get; } = new();

    [JsonPropertyName("timestamp")]
    [BsonElement("timestamp")]
    public long Timestamp { set; get; } = 0;

    [JsonPropertyName("name")]
    [BsonElement("name")]
    public string Name { set; get; } = "";

    [JsonPropertyName("email")]
    [BsonElement("email")]
    public string Email { set; get; } = "";

    [JsonPropertyName("message")]
    [BsonElement("message")]
    public string Message { set; get; } = "";

    [JsonPropertyName("acceptedTerms")]
    [BsonElement("acceptedTerms")]
    public bool AcceptedTerms { set; get; } = false;

    [JsonPropertyName("acceptedTermsAt")]
    [BsonElement("acceptedTermsAt")]
    public long AcceptedTermsAt { set; get; } = 0;

    [JsonPropertyName("captchaScore")]
    [BsonElement("captchaScore")]
    public double CaptchaScore { set; get; } = 0;

    [JsonPropertyName("origin")]
    [BsonElement("origin")]
    public string Origin { set; get; } = "";

    [JsonIgnore]
    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}