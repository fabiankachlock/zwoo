using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Database;

namespace Zwoo.Backend.Shared.Services;

public class ZwooDatabaseOptions
{
    public bool EnableMigrations { get; set; }
}

public static class DatabaseExtensions
{
    public static void AddZwooDatabase(this WebApplicationBuilder builder, ZwooOptions conf, ZwooDatabaseOptions options)
    {
        var db = new Database.Database(conf.Database.ConnectionUri, conf.Database.DBName);

        builder.Services.AddSingleton<IDatabase>(db);
        builder.Services.AddSingleton<IAuditTrailService, AuditTrailService>();
        builder.Services.AddSingleton<IAccountEventService, AccountEventService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IBetaCodesService, BetaCodesService>();
        builder.Services.AddSingleton<IChangelogService, ChangelogService>();
        builder.Services.AddSingleton<IGameInfoService, GameInfoService>();
        builder.Services.AddSingleton<IContactRequestService, ContactRequestService>();

        builder.Services.AddSingleton(db.Client);
        builder.Services.Configure<MongoMigrationSettings>(options =>
        {
            options.ConnectionString = conf.Database.ConnectionUri;
            options.Database = conf.Database.DBName;
            options.DatabaseMigrationVersion = new DocumentVersion(conf.App.AppVersion);
        });

        if (options.EnableMigrations)
        {
            builder.Services.AddMigration(new MongoMigrationSettings
            {
                ConnectionString = conf.Database.ConnectionUri,
                Database = conf.Database.DBName,
                DatabaseMigrationVersion = new DocumentVersion(conf.App.AppVersion)
            });
        }

        Mongo.Migration.DocumentVersionSerializer.DefaultVersion = conf.App.AppVersion;
    }
}
