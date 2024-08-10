using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Zwoo.Backend.Shared.Configuration;

namespace Zwoo.Backend.Shared.Authentication;

public static class AppExtensions
{
    /// <summary>
    /// add the default zwoo authentication mechanism to the api
    /// </summary>
    /// <param name="services">a service collection</param>
    /// <param name="options">the current configuration</param>
    public static void AddZwooAuthentication(this IServiceCollection services, ZwooOptions options)
    {
        // configure cookie based authentication
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
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
                o.Cookie.SameSite = SameSiteMode.Strict;
            }
            o.Cookie.Domain = options.Server.CookieDomain;
        });
        // add authorization
        services.AddAuthorization(o =>
        {
            // require an active cookie session as default policy
            o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });
        // register cookie authentication events handler in di container 
        services.AddScoped<ZwooCookieAuthenticationEvents>();
    }

    /// <summary>
    /// use the default zwoo authentication mechanism
    /// </summary>
    /// <param name="app">the current web application</param>
    public static void UseZwooAuthentication(this WebApplication app, IEndpointRouteBuilder? route = null)
    {

        app.UseAuthentication();
        app.UseAuthorization();
        AuthenticationEndpoints.Map(route ?? app);
    }
}