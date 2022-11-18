using System.Text.Json.Serialization;
using Mongo.Migration;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseClasses;

[RuntimeVersion("1.0.0-beta.7")]
[StartUpVersion("1.0.0-beta.7")]
[CollectionLocation("changelogs", "zwoo")]
public partial class Changelog : IDocument
{
    public Changelog() {}

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

    [JsonIgnore]
    [BsonElement("_id")]
    public ObjectId Id { set; get; }
    
    [JsonPropertyName("version")]
    [BsonElement("changelog_version")]
    public string ChangelogVersion { set; get; } = "";

    [JsonPropertyName("changelog")]
    [BsonElement("changelog")]
    public string ChangelogText { set; get; } = "";

    [JsonIgnore]
    [BsonElement("public")] 
    public bool Public { set; get; } = false;

    [JsonPropertyName("timestamp")]
    [BsonElement("timestamp")]
    public ulong Timestamp { set; get; } = 0;

    [BsonIgnore]
    [JsonIgnore]
    public bool Private
    {
        set => Public = !value;
        get => !Public;
    }

    [JsonIgnore]
    [BsonElement("version")]
    [BsonSerializer(typeof(DocumentVersionSerializer))]
    public DocumentVersion Version { get; set; }
}