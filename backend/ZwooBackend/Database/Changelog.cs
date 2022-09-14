using System.Security;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooBackend.Database;

public class Changelog
{
    public Changelog() {}

    [BsonConstructor]
    public Changelog(string version, string changelogText)
    {
        Version = version;
        ChangelogText = changelogText;
    }

    [BsonElement("version")]
    public string Version { set; get; } = "";

    [BsonElement("changelog")]
    public string ChangelogText { set; get; } = "";
}