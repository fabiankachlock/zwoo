using MongoDB.Driver;

namespace ZwooBackend.Database;

public class Database
{
    public Database()
    {
        _client = new MongoClient(Environment.GetEnvironmentVariable("ZWOO_DATABASE_CONNECTION_STRING"));
        Globals.Logger.Info($"Established MongoDB Connection to {Environment.GetEnvironmentVariable("ZWOO_DATABASE_CONNECTION_STRING")}");

        _database = _client.GetDatabase("zwoo");
    }
    
    
    private MongoClient _client;
    private IMongoDatabase _database;
}