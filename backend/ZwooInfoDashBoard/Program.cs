using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using ZwooInfoDashBoard.Data;
using Mongo.Migration.Documents;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using ZwooInfoDashBoard;
using ZwooDatabase;
using Radzen;
using Microsoft.IdentityModel.Logging;

IdentityModelEventSource.ShowPII = true;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.WebHost.UseStaticWebAssets();
builder.Services.AddScoped<DialogService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClaimsTransformation, KeycloakRolesClaimsTransformation>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = Globals.AuthenticationAuthority;
    options.ClientId = Globals.AuthenticationClientId;
    options.ClientSecret = Globals.AuthenticationClientSecret;

    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false;

    options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("roles");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
    };

    options.Events = new OpenIdConnectEvents
    {
        OnAccessDenied = context =>
        {
            context.HandleResponse();
            context.Response.Redirect("/");
            return Task.CompletedTask;
        },
    };
});


// database
var db = new ZwooDatabase.Database(Globals.ConnectionString, Globals.DatabaseName);

builder.Services.AddSingleton<IDatabase>(db);
builder.Services.AddSingleton<IBetaCodesService, BetaCodesService>();
builder.Services.AddSingleton<IAuditTrailService, AuditTrailService>();
builder.Services.AddSingleton<IAccountEventService, AccountEventService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IChangelogService, ChangelogService>();
builder.Services.AddSingleton<IGameInfoService, GameInfoService>();

// migrations
builder.Services.AddSingleton(db.Client);
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

// services
builder.Services.AddSingleton<IAuthService, AuthService>();


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

app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always
});

app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
