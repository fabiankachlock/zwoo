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
                new User(p.Id, p.Sid, p.Username, p.Email, p.Password, p.Wins, p.Settings, p.ValidationCode, p.Verified));
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
                new Changelog(c.Id, c.ChangelogVersion, c.ChangelogText, c.Public, c.Timestamp));
        });

        BsonClassMap.RegisterClassMap<PlayerScore>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new PlayerScore(p.PlayerUsername, p.Score, p.IsBot));
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

        BsonClassMap.RegisterClassMap<ContactRequest>(cm =>
        {
            cm.AutoMap();
            cm.MapCreator(p =>
                new ContactRequest(p.Id, p.Timestamp, p.Name, p.Email, p.Message, p.CaptchaScore, p.Origin));
        });
    }
}