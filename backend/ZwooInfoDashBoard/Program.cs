using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ZwooInfoDashBoard.Data;
using Mongo.Migration.Documents;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<DialogService>();

builder.WebHost.UseStaticWebAssets();

// Database
builder.Services.AddSingleton(Globals.ZwooDatabase._client);
builder.Services.Configure<MongoMigrationSettings>(options =>
{
    options.ConnectionString = Globals.ConnectionString;
    options.Database = "zwoo";
    options.DatabaseMigrationVersion = new DocumentVersion(Globals.Version);
});
builder.Services.AddMigration(new MongoMigrationSettings
{
    ConnectionString = Globals.ConnectionString,
    Database = Globals.DatabaseName,
    DatabaseMigrationVersion = new DocumentVersion(Globals.Version)
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
