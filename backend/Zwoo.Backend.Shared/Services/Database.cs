using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mongo.Migration.Documents;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Driver;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Database;

namespace Zwoo.Backend.Shared.Services;

public class ZwooDatabaseOptions
{
    public bool EnableMigrations { get; set; }
}

public static class DatabaseExtensions
{
    public static void AddZwooDatabase(this IServiceCollection services, ZwooOptions conf, ZwooDatabaseOptions options)
    {


        services.AddSingleton<IDatabase>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Database.Database>>();
            var db = new Database.Database(conf.Database.ConnectionUri, conf.Database.DBName, logger);
            return db;
        });
        services.AddSingleton<IAuditTrailService, AuditTrailService>();
        services.AddSingleton<IAccountEventService, AccountEventService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IBetaCodesService, BetaCodesService>();
        services.AddSingleton<IChangelogService, ChangelogService>();
        services.AddSingleton<IGameInfoService, GameInfoService>();
        services.AddSingleton<IContactRequestService, ContactRequestService>();

        services.AddSingleton<IMongoClient>(sp =>
        {
            var db = sp.GetRequiredService<IDatabase>();
            return db.Client;
        });
        services.Configure<MongoMigrationSettings>(options =>
        {
            options.ConnectionString = conf.Database.ConnectionUri;
            options.Database = conf.Database.DBName;
            options.DatabaseMigrationVersion = new DocumentVersion(conf.App.AppVersion);
        });

        if (options.EnableMigrations)
        {
            services.AddMigration(new MongoMigrationSettings
            {
                ConnectionString = conf.Database.ConnectionUri,
                Database = conf.Database.DBName,
                DatabaseMigrationVersion = new DocumentVersion(conf.App.AppVersion)
            });
        }

        Mongo.Migration.DocumentVersionSerializer.DefaultVersion = conf.App.AppVersion;
    }
}
