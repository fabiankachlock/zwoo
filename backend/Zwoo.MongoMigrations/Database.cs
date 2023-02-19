using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Zwoo.MongoMigrations;

public class DB100Beta7 : DatabaseMigration
{
    public DB100Beta7() : base("1.0.0-beta.7")
    {
    }

    public override void Up(IMongoDatabase db)
    {
        Console.WriteLine("Up");
    }

    public override void Down(IMongoDatabase db)
    {
        Console.WriteLine("Down");
    }
}

public class DB100Beta8 : DatabaseMigration
{
    public DB100Beta8() : base("1.0.0-beta.8")
    {
    }

    public override void Up(IMongoDatabase db)
    {
        Console.WriteLine("Up");
    }

    public override void Down(IMongoDatabase db)
    {
        Console.WriteLine("Down");
    }
}