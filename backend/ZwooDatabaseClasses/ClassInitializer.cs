using MongoDB.Bson.Serialization;

namespace ZwooDatabaseClasses;

public static class ClassInitializer
{
    static ClassInitializer()
    {
        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new User(p.Id, p.Sid, p.Username, p.Email, p.Password, p.Wins, p.ValidationCode, p.Verified));
        });
        
        BsonClassMap.RegisterClassMap<BetaCode>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new BetaCode(p.Id, p.Code));
        });
        
        BsonClassMap.RegisterClassMap<Changelog>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(c =>
                new Changelog(c.Id, c.Version, c.ChangelogText, c.Public));
        });
        
        BsonClassMap.RegisterClassMap<PlayerScore>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new PlayerScore(p.PlayerUsername, p.Score));
        });
        
        BsonClassMap.RegisterClassMap<GameInfo>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new GameInfo(p.Id, p.GameName, p.GameId, p.IsPublic, p.Scores, p.TimeStamp));
        });

        BsonClassMap.RegisterClassMap<AccountEvent>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new AccountEvent(p.Id, p.EventType, p.PlayerID, p.Success, p.TimeStamp));
        });
    }
}