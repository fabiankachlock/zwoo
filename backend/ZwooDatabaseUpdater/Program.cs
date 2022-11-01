using MongoDB.Driver;
using ZwooDatabaseClasses;

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ZWOO_DATABASE_CONNECTION_STRING")))
{
    Console.WriteLine("No Connection string specified");
    Environment.Exit(1);
}
var client = new MongoClient(Environment.GetEnvironmentVariable("ZWOO_DATABASE_CONNECTION_STRING"));
var database = client.GetDatabase("zwoo");

// --------------------------------------------------------------------------------------------------------------------//

var changelogCollectionLegacy = database.GetCollection<ChangelogLegacy>("changelogs");

var changelogsList = new List<Changelog>();
foreach (var u in changelogCollectionLegacy.AsQueryable())
{
    changelogsList.Add(new Changelog(u.Id, u.Version, u.ChangelogText, false, 1));
}

database.DropCollection("changelogs");
var gameInfoCollection = database.GetCollection<Changelog>("changelogs");
gameInfoCollection.InsertMany(changelogsList);