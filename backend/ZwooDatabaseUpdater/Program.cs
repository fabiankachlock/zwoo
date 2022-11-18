using Mongo.Migration.Documents;
using MongoDB.Bson;
using MongoDB.Driver;
using ZwooDatabaseClasses;
using ZwooDatabaseUpdater;

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ZWOO_DATABASE_CONNECTION_STRING")))
{
    Console.WriteLine("No Connection string specified");
    Environment.Exit(1);
}
var client = new MongoClient(Environment.GetEnvironmentVariable("ZWOO_DATABASE_CONNECTION_STRING"));
var database = client.GetDatabase("zwoo");

// -------------------------------------------------------------------------------------------------------------------- //

var users_old = database.GetCollection<BsonDocument>("users");

var users = new List<BsonDocument>();
foreach (var u in users_old.AsQueryable())
{
    u.SetElement(new BsonElement("settings", ""));
    u.SetElement(new BsonElement("version", "1.0.0-beta.7"));
    users.Add(u);
}

database.DropCollection("users");
var usercoll = database.GetCollection<BsonDocument>("users");
usercoll.InsertMany(users);

// -------------------------------------------------------------------------------------------------------------------- //

var changelogCollectionLegacy = database.GetCollection<ChangelogL>("changelogs");

var changelogs = new List<Changelog>();
foreach (var c in changelogCollectionLegacy.AsQueryable())
{
    var ch = new Changelog(c.Id, c.Version, c.ChangelogText, c.Public, c.Timestamp);
    ch.Version = new DocumentVersion("1.0.0-beta.7");
    changelogs.Add(ch);
}

database.DropCollection("changelogs");
var changelogCollection = database.GetCollection<Changelog>("changelogs");
changelogCollection.InsertMany(changelogs);

// -------------------------------------------------------------------------------------------------------------------- //

var games_info_old = database.GetCollection<BsonDocument>("game_info");

var games = new List<BsonDocument>();
foreach (var g in games_info_old.AsQueryable())
{
    g.SetElement(new BsonElement("version", "1.0.0-beta.7"));
    games.Add(g);
}

database.DropCollection("game_info");
var games_collection = database.GetCollection<BsonDocument>("game_info");
games_collection.InsertMany(games);

// -------------------------------------------------------------------------------------------------------------------- //
var ae = database.GetCollection<BsonDocument>("account_events");

var account_events = new List<BsonDocument>();
foreach (var g in ae.AsQueryable())
{
    g.SetElement(new BsonElement("version", "1.0.0-beta.7"));
    account_events.Add(g);
}

database.DropCollection("account_events");
var aeColl = database.GetCollection<BsonDocument>("account_events");
aeColl.InsertMany(account_events);