using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;
using ZwooDatabase.Dao;

namespace Zwoo.MongoMigrations;

public class AuditTrailPayloadMigration : DocumentMigration<AuditTrailDao>
{
    private readonly string currentType;
    private readonly string replacement;

    public AuditTrailPayloadMigration(string type, string replacement, string version) : base(version)
    {
        this.currentType = type;
        this.replacement = replacement;
    }

    public override void Up(BsonDocument document)
    {
        foreach (var e in document["events"].AsBsonArray)
        {
            if (e["newValue"].BsonType != BsonType.Null && e["newValue"]["_t"] == currentType)
            {
                e["newValue"]["_t"] = replacement;
            }
            if (e["oldValue"].BsonType != BsonType.Null && e["oldValue"]["_t"] == currentType)
            {
                e["oldValue"]["_t"] = replacement;
            }
        }
    }

    public override void Down(BsonDocument document)
    {
        foreach (var e in document["events"].AsBsonArray)
        {
            if (e["newValue"].BsonType != BsonType.Null && e["newValue"]["_t"] == replacement)
            {
                e["newValue"]["_t"] = currentType;
            }
            if (e["oldValue"].BsonType != BsonType.Null && e["oldValue"]["_t"] == replacement)
            {
                e["oldValue"]["_t"] = currentType;
            }
        }
    }
}

public class Beta10012AuditTrail : AuditTrailPayloadMigration
{
    public Beta10012AuditTrail() : base("UserDao", "Beta11UserDao", "1.0.0-beta.12")
    {
    }
}


public class Beta10015AuditTrail : AuditTrailPayloadMigration
{
    public Beta10015AuditTrail() : base("UserDao", "Beta12UserDao", "1.0.0-beta.15")
    {
    }
}