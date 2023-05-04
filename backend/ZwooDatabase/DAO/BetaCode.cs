using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Mongo.Migration.Documents.Attributes;


namespace ZwooDatabase.Dao;

[CollectionLocation("betacodes")]
public partial class BetaCodeDao
{
    BetaCodeDao() { }

    public BetaCodeDao(string code) => Code = code;

    [BsonConstructor]
    public BetaCodeDao(ObjectId id, string code)
    {
        Id = id;
        Code = code;
    }

    [BsonElement("_id")]
    public ObjectId Id { set; get; }

    [BsonElement("code")]
    public string Code { set; get; } = null!;
}