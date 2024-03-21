using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;
using Zwoo.Database.Dao;

namespace Zwoo.Migrations;

public class Beta1008PlayerScore : DocumentMigration<GameInfoDao>
{
    public Beta1008PlayerScore() : base("1.0.0-beta.8")
    {
    }

    public override void Up(BsonDocument document)
    {
        foreach (BsonDocument doc in (BsonArray)document["scores"])
        {
            doc.Add("is_bot", false);
        }
    }

    public override void Down(BsonDocument document)
    {
        foreach (BsonDocument doc in (BsonArray)document["scores"])
        {
            doc.Remove("is_bot");
        }
    }
}