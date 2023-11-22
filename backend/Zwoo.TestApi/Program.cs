using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Zwoo.Backend.Shared.Api;
using Zwoo.Backend.Shared.Api.Discover;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conf = builder.AddZwooConfiguration(args, new ZwooAppConfiguration()
{
    AppVersion = "test"
});
builder.AddZwooCors(conf);

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
app.UseDiscover();

app.MapGet("/test", (IOptionsSnapshot<ZwooOptions> conf) =>
{
    return Results.Ok(conf.Value);
});

app.Run();

class TestImpl : IDiscoverService
{
    public bool CanConnect(ClientInfo client)
    {
        return Random.Shared.NextSingle() < 0.5;
    }
}