using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZwooDatabaseClasses;

public partial class BetaCode
{
    BetaCode() { }

    public BetaCode(string code) => Code = code;

    [BsonConstructor]
    public BetaCode(ObjectId id, string code)
    {
        Id = id;
        Code = code;
    }

    [BsonElement("_id")]
    public ObjectId Id { set; get; }

    [BsonElement("code")]
    public string Code { set; get; } = null!;
}

public partial class User
{


    [BsonElement("")]
    public string Name { get; set; }
}