using Microsoft.VisualBasic.FileIO;
using MongoDB.Bson;
using MongoDB.Driver;

var file_path = Environment.GetCommandLineArgs()[1];

List<string> codes = new List<string>();

using (TextFieldParser reader = new TextFieldParser(file_path))
{
    reader.Delimiters = new string[] { "," };
    reader.ReadFields();
    while (!reader.EndOfData)
        codes.Add(reader.ReadFields()?[1]!);
}

var client = new MongoClient(Environment.GetCommandLineArgs()[2]);

var _database = client.GetDatabase("zwoo");
var _collection = _database.GetCollection<BsonDocument>("betacodes");

_collection.InsertMany(codes.Select(x => new BsonDocument { {"code", x} } ));