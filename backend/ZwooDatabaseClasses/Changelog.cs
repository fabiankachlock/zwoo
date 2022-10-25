using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseClasses;

public partial class Changelog
{
    public Changelog() {}

    public Changelog(string version, string changelogText, bool @public)
    {
        Version = version;
        ChangelogText = changelogText;
        Public = @public;
    }
    
    [BsonConstructor]
    public Changelog(ObjectId id, string version, string changelogText, bool @public)
    {
        Id = id;
        Version = version;
        ChangelogText = changelogText;
        Public = @public;
    }

    [BsonElement("_id")]
    public ObjectId Id { set; get; }
    
    [BsonElement("version")]
    public string Version { set; get; } = "";

    [BsonElement("changelog")]
    public string ChangelogText { set; get; } = "";

    [BsonElement("public")] 
    public bool Public { set; get; } = false;
}