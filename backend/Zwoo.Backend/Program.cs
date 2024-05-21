using Quartz;
using Zwoo.Backend.Services;
using Zwoo.Backend.Shared.Services;
using Zwoo.Backend.Shared.Api;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Backend.Shared.Authentication;
using Zwoo.Backend.Shared.Api.Discover;
using Zwoo.Backend.Shared.Api.Contact;
using Zwoo.Backend.Shared.Api.Game;
using Mongo.Migration.Migrations.Document;
using Zwoo.Database.Dao;
using Zwoo.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string VERSION = "1.0.0-beta.18";
builder.AddZwooLogging(false);
var conf = builder.AddZwooConfiguration(args, new ZwooAppConfiguration()
{
    AppVersion = VERSION,
    ServerMode = "online"
});
builder.Services.AddZwooCors(conf);
builder.Services.AddZwooAuthentication(conf);
builder.Services.AddZwooDatabase(conf, new ZwooDatabaseOptions()
{
    EnableMigrations = true
});

// var documentMigrations = typeof(DB100Beta7)
//     .Assembly
//     .GetTypes()
//     .Where(t => t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(DocumentMigration<>))
//     .Where(t => t.BaseType?.GetGenericArguments()[0] == typeof(UserDao))
//     .ToList();

// Console.WriteLine($"Found {documentMigrations.Count} document migrations.");
// foreach (var migration in documentMigrations)
// {
//     builder.Services.AddSingleton(typeof(IDocumentMigration), migration);
// }


// backend services
builder.Services.AddZwooServices();
builder.Services.AddGameServices();
builder.Services.AddGameDatabaseAdapter();

// scheduler
builder.Services.AddZwooScheduler(q =>
{
    q.ScheduleJob<DatabaseCleanupJob>(t => t
                .WithIdentity("cleanup", "db")
                .WithCronSchedule("0 1 1 1/1 * ? *"));
});
builder.Services.AddTransient<DatabaseCleanupJob>();

var app = builder.Build();

app.UseZwooHttpLogging();
app.UseZwooCors();
app.UseZwooAuthentication();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
webSocketOptions.AllowedOrigins.Add(conf.Server.Cors);

app.UseWebSockets(webSocketOptions);
app.MapControllers();
app.UseDiscover();
app.UseContactRequests();
app.UseGame();

app.Run();
