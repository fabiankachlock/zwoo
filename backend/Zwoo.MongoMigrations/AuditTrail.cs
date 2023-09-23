using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;
using ZwooDatabase.Dao;

namespace Zwoo.MongoMigrations;

public class Beta10012AuditTrail : DocumentMigration<AuditTrailDao>
{
    public Beta10012AuditTrail() : base("1.0.0-beta.12")
    {
    }

    public override void Up(BsonDocument document)
    {
        foreach (var e in document["events"].AsBsonArray)
        {
            if (e["newValue"].BsonType != BsonType.Null && e["newValue"]["_t"] == "UserDao")
            {
                e["newValue"]["_t"] = "Beta11UserDao";
            }
            if (e["oldValue"].BsonType != BsonType.Null && e["oldValue"]["_t"] == "UserDao")
            {
                e["oldValue"]["_t"] = "Beta11UserDao";
            }
        }
    }

    public override void Down(BsonDocument document)
    {
        foreach (var e in document["events"].AsBsonArray)
        {
            if (e["newValue"].BsonType != BsonType.Null && e["newValue"]["_t"] == "Beta11UserDao")
            {
                e["newValue"]["_t"] = "UserDao";
            }
            if (e["oldValue"].BsonType != BsonType.Null && e["oldValue"]["_t"] == "Beta11UserDao")
            {
                e["oldValue"]["_t"] = "UserDao";
            }
        }
    }
}


public class Beta10015AuditTrail : DocumentMigration<AuditTrailDao>
{
    public Beta10015AuditTrail() : base("1.0.0-beta.15")
    {
    }

    public override void Up(BsonDocument document)
    {
        foreach (var e in document["events"].AsBsonArray)
        {
            if (e["newValue"].BsonType != BsonType.Null && e["newValue"]["_t"] == "UserDao")
            {
                e["newValue"]["_t"] = "Beta12UserDao";
            }
            if (e["oldValue"].BsonType != BsonType.Null && e["oldValue"]["_t"] == "UserDao")
            {
                e["oldValue"]["_t"] = "Beta12UserDao";
            }
        }
    }

    public override void Down(BsonDocument document)
    {
        foreach (var e in document["events"].AsBsonArray)
        {
            if (e["newValue"].BsonType != BsonType.Null && e["newValue"]["_t"] == "Beta12UserDao")
            {
                e["newValue"]["_t"] = "UserDao";
            }
            if (e["oldValue"].BsonType != BsonType.Null && e["oldValue"]["_t"] == "Beta12UserDao")
            {
                e["oldValue"]["_t"] = "UserDao";
            }
        }
    }
}