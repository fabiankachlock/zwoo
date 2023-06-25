using System.Text.Json.Serialization;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ZwooDatabase.Dao;


[RuntimeVersion("1.0.0-beta.11")]
[StartUpVersion("1.0.0-beta.11")]
[CollectionLocation("contact_request")]
public class ContactRequest : IDocument
{
    public ContactRequest() { }

    [BsonConstructor]
    public ContactRequest(ObjectId id, long timestamp, string name, string email, string message, double captchaScore, string origin)
    {
        Id = id;
        Timestamp = timestamp;
        Name = name;
        Email = email;
        Message = message;
        CaptchaScore = captchaScore;
        Origin = origin;
    }

    [BsonElement("_id")]
    public ObjectId Id { set; get; } = new();

    [BsonElement("timestamp")]
    public long Timestamp { set; get; } = 0;

    [BsonElement("name")]
    public string Name { set; get; } = "";

    [BsonElement("email")]
    public string Email { set; get; } = "";

    [BsonElement("message")]
    public string Message { set; get; } = "";

    [BsonElement("captchaScore")]
    public double CaptchaScore { set; get; } = 0;

    [BsonElement("origin")]
    public string Origin { set; get; } = "";

    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}