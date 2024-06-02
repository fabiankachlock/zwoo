using Mongo.Migration;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Zwoo.Database.Dao;

[RuntimeVersion("1.0.0-beta.18")]
[StartUpVersion("1.0.0-beta.18")]
[CollectionLocation("audit_trails")]
public partial class AuditTrailDao : IDocument
{
    public AuditTrailDao() { }

    [BsonConstructor]
    public AuditTrailDao(string id, List<AuditEventDao> events)
    {
        Id = id;
        Events = events;
    }

    [BsonElement("_id")]
    public string Id { set; get; } = "";

    [BsonElement("events")]
    public List<AuditEventDao> Events { get; set; } = new();


    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}