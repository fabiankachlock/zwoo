using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;
using Zwoo.Database.Dao;

namespace Zwoo.Migrations;

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


public class Beta10016AuditTrail : DocumentMigration<AuditTrailDao>
{
    public Beta10016AuditTrail() : base("1.0.0-beta.16")
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

public class Beta10018AuditTrail : DocumentMigration<AuditTrailDao>
{
    public Beta10018AuditTrail() : base("1.0.0-beta.18")
    {
    }

    public override void Up(BsonDocument document)
    {
        foreach (var e in document["events"].AsBsonArray)
        {
            if (e["newValue"].BsonType != BsonType.Null && e["newValue"]["_t"] == "UserDao")
            {
                e["newValue"]["_t"] = "Beta17UserDao";
            }
            if (e["oldValue"].BsonType != BsonType.Null && e["oldValue"]["_t"] == "UserDao")
            {
                e["oldValue"]["_t"] = "Beta17UserDao";
            }
        }
    }

    public override void Down(BsonDocument document)
    {
        foreach (var e in document["events"].AsBsonArray)
        {
            if (e["newValue"].BsonType != BsonType.Null && e["newValue"]["_t"] == "Beta17UserDao")
            {
                e["newValue"]["_t"] = "UserDao";
            }
            if (e["oldValue"].BsonType != BsonType.Null && e["oldValue"]["_t"] == "Beta17UserDao")
            {
                e["oldValue"]["_t"] = "UserDao";
            }
        }
    }
}