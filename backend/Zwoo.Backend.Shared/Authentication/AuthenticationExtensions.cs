using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Database;

namespace Zwoo.Backend.Shared.Authentication;

public static class AppExtensions
{
    /// <summary>
    /// add the default zwoo authentication mechanism to the api
    /// </summary>
    /// <param name="builder">the web application builder</param>
    /// <param name="options">the current configuration</param>
    public static void AddZwooAuthentication(this WebApplicationBuilder builder, ZwooOptions options)
    {
        // configure cookie based authentication
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
        {
            // Dont use SlidingExpiration, because its an security issue!
            o.ExpireTimeSpan = TimeSpan.FromDays(90);
            // react to authentication events for eg. session validation
            o.EventsType = typeof(ZwooCookieAuthenticationEvents);
            // cookie settings
            o.Cookie.Name = "auth";
            o.Cookie.HttpOnly = true;
            o.Cookie.MaxAge = o.ExpireTimeSpan;
            if (options.Server.UseSsl)
            {
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.SameSite = SameSiteMode.None;
            }
            o.Cookie.Domain = options.Server.CookieDomain;
        });
        // add authorization
        builder.Services.AddAuthorization(o =>
        {
            // require an active cookie session as default policy
            o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });
        // register cookie authentication events handler in di container 
        builder.Services.AddScoped<ZwooCookieAuthenticationEvents>();
    }

    /// <summary>
    /// use the default zwoo authentication mechanism
    /// </summary>
    /// <param name="app">the current web application</param>
    public static void UseZwooAuthentication(this WebApplication app)
    {

        app.UseAuthentication();
        app.UseAuthorization();
        AuthenticationEndpoints.Map(app);
    }
}