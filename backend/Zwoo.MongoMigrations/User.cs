using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;
using ZwooDatabase.Dao;

namespace Zwoo.MongoMigrations;

public class Beta1007User : DocumentMigration<UserDao>
{
    public Beta1007User() : base("1.0.0-beta.7")
    {
    }

    public override void Up(BsonDocument document)
    {
        document.Add("settings", "");
    }

    public override void Down(BsonDocument document)
    {
        document.Remove("settings");
    }
}

public class Beta10012User : DocumentMigration<UserDao>
{
    public Beta10012User() : base("1.0.0-beta.12")
    {
    }

    public override void Up(BsonDocument document)
    {
        document.Add("accepted_terms", true);
    }

    public override void Down(BsonDocument document)
    {
        document.Remove("accepted_terms");
    }
}