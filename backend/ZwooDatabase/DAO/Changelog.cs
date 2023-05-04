using Mongo.Migration;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabase.Dao;

[RuntimeVersion("1.0.0-beta.7")]
[StartUpVersion("1.0.0-beta.7")]
[CollectionLocation("changelogs")]
public partial class Changelog : IDocument
{
    public Changelog() { }

    public Changelog(string changelogVersion, string changelogText, bool @public)
    {
        ChangelogVersion = changelogVersion;
        ChangelogText = changelogText;
        Public = @public;
    }

    [BsonConstructor]
    public Changelog(ObjectId id, string changelogVersion, string changelogText, bool @public, ulong timestamp)
    {
        Id = id;
        ChangelogVersion = changelogVersion;
        ChangelogText = changelogText;
        Public = @public;
        Timestamp = timestamp;
    }

    [BsonElement("_id")]
    public ObjectId Id { set; get; }

    [BsonElement("changelog_version")]
    public string ChangelogVersion { set; get; } = "";

    [BsonElement("changelog")]
    public string ChangelogText { set; get; } = "";

    [BsonElement("public")]
    public bool Public { set; get; } = false;

    [BsonElement("timestamp")]
    public ulong Timestamp { set; get; } = 0;

    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}