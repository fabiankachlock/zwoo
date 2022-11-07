using System.Text.Json.Serialization;
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
    public Changelog(ObjectId id, string version, string changelogText, bool @public, ulong timestamp)
    {
        Id = id;
        Version = version;
        ChangelogText = changelogText;
        Public = @public;
        Timestamp = timestamp;
    }

    [JsonIgnore]
    [BsonElement("_id")]
    public ObjectId Id { set; get; }
    
    [JsonPropertyName("version")]
    [BsonElement("version")]
    public string Version { set; get; } = "";

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

}