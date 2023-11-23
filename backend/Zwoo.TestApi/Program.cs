using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Mongo.Migration.Documents;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Zwoo.Backend.Shared.Api;
using Zwoo.Backend.Shared.Api.Discover;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Authentication;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Backend.Shared.Services;
using Zwoo.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conf = builder.AddZwooConfiguration(args, new ZwooAppConfiguration()
{
    AppVersion = "1.0.0-beta.17"
});
builder.AddZwooCors(conf);
builder.AddZwooDatabase(conf, new ZwooDatabaseOptions
{
    EnableMigrations = false
});
builder.AddZwooAuthentication(conf);

builder.Services.AddSingleton<IDiscoverService, TestImpl>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseZwooCors();
app.UseZwooAuthentication();
app.UseDiscover();

// IOptionsSnapshot<ZwooOptions> conf
app.MapGet("/version", () =>
{
    return conf.App.AppVersion;
}).AllowAnonymous();
app.Run();

class TestImpl : IDiscoverService
{
    public bool CanConnect(ClientInfo client)
    {
        return Random.Shared.NextSingle() < 0.5;
    }
}