using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseClasses;

public partial class Changelog
{
    public Changelog() {}

    public Changelog(string version, string changelogText)
    {
        Version = version;
        ChangelogText = changelogText;
    }
    
    [BsonConstructor]
    public Changelog(ObjectId id, string version, string changelogText)
    {
        Id = id;
        Version = version;
        ChangelogText = changelogText;
    }

    [BsonElement("_id")]
    public ObjectId Id { set; get; }
    
    [BsonElement("version")]
    public string Version { set; get; } = "";

    [BsonElement("changelog")]
    public string ChangelogText { set; get; } = "";
}