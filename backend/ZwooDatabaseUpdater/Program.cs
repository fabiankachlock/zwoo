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

var userCollectionLegacy = database.GetCollection<UserLegacy>("users");

var userInfos = new List<User>();
foreach (var u in userCollectionLegacy.AsQueryable())
{
    userInfos.Add(new User(u.Id, new List<string> { u.Sid }, u.Username, u.Email, u.Password, u.Wins, u.ValidationCode, u.Verified));
}

database.DropCollection("users");
var gameInfoCollection = database.GetCollection<User>("users");
gameInfoCollection.InsertMany(userInfos);