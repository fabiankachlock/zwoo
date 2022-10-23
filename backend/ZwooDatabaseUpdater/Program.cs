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

// --------------------------------------------------------------------------------------------------------------------//

var userCollection = database.GetCollection<User>("users");
var gameInfoOldCollection = database.GetCollection<GameInfoLegacy>("game_info");

var newGameInfos = new List<GameInfo>();
foreach (var game in gameInfoOldCollection.AsQueryable())
{
    var scores = new List<PlayerScore>();
    foreach (var score in game.Scores)
    {
        var user = userCollection.AsQueryable().FirstOrDefault(x => x.Id == (ulong)score.PlayerID);
        scores.Add(user == null
            ? new PlayerScore("No User found", score.Score)
            : new PlayerScore(user.Username, score.Score)
        );
    }
    newGameInfos.Add(new GameInfo(game.Id, game.GameName, game.GameID, game.IsPublic, scores, game.TimeStamp));
}

database.DropCollection("game_info");
var gameInfoCollection = database.GetCollection<GameInfo>("game_info");
gameInfoCollection.InsertMany(newGameInfos);