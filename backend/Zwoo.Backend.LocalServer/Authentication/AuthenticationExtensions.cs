using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zwoo.Backend.Shared.Configuration;

namespace Zwoo.Backend.LocalServer.Authentication;

public static class AppExtensions
{
    public static void AddLocalAuthentication(this IServiceCollection services, ZwooOptions options, string serverId)
    {
        // configure cookie based authentication
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
        {
            // Dont use SlidingExpiration, because its an security issue!
            o.ExpireTimeSpan = TimeSpan.FromDays(90);
            o.EventsType = typeof(LocalServerAuthenticationEvents);
            o.Cookie.Name = $"localSever#{serverId}";
            o.Cookie.HttpOnly = true;
            o.Cookie.MaxAge = o.ExpireTimeSpan;
            if (options.Server.UseSsl)
            {
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.SameSite = SameSiteMode.None;
            }
            o.Cookie.Domain = options.Server.CookieDomain;
        });
        services.AddAuthorization(o =>
        {
            o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });
        services.AddScoped<LocalServerAuthenticationEvents>();
    }

    public static void UseLocalAuthentication(this WebApplication app)
    {

        app.UseAuthentication();
        app.UseAuthorization();
        LocalAuthenticationEndpoints.Map(app);
    }
}