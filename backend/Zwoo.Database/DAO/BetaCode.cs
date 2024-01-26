using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Mongo.Migration.Documents.Attributes;


namespace Zwoo.Database.Dao;

[CollectionLocation("betacodes")]
public partial class BetaCodeDao
{
    public BetaCodeDao()
    {
        Id = ObjectId.GenerateNewId();
    }

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