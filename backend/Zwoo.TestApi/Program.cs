using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Zwoo.Backend.Shared;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDiscover();

app.MapGet("/test", (IOptionsSnapshot<ZwooOptions> conf) =>
{
    return Results.Ok(conf.Value);
});

app.Run();

