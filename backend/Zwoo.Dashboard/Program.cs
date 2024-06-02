using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Zwoo.Dashboard.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Zwoo.Dashboard;
using Radzen;
using Microsoft.IdentityModel.Logging;
using Zwoo.Backend.Shared.Services;
using Zwoo.Backend.Shared.Configuration;

IdentityModelEventSource.ShowPII = true;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.WebHost.UseStaticWebAssets();
builder.Services.AddScoped<DialogService>();

const string VERSION = "1.0.0-beta.18";
builder.AddZwooLogging(false);
var zwooConf = builder.AddZwooConfiguration(args, new() { });
var conf = builder.AddZiadConfiguration(args, new ZiadAppConfiguration()
{
    AppVersion = VERSION
});

Console.WriteLine($"Zwoo Dashboard v{conf.App.AppVersion}");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClaimsTransformation, KeycloakRolesClaimsTransformation>();
builder.Services
    .AddAuthentication(options =>
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
        options.Authority = conf.Auth.Authority;
        options.ClientId = conf.Auth.ClientID;
        options.ClientSecret = conf.Auth.ClientSecret;

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


builder.Services.AddZwooDatabase(zwooConf, new ZwooDatabaseOptions()
{
    EnableMigrations = false
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

// app.UseHttpsRedirection();
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
