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
    public static void AddLocalAuthentication(this IServiceCollection services, string serverId)
    {
        // configure cookie based authentication
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
        {
            o.ExpireTimeSpan = TimeSpan.FromDays(1);
            o.EventsType = typeof(LocalServerAuthenticationEvents);
            o.Cookie.Name = $"localSever__{serverId}";
            o.Cookie.HttpOnly = true;
            o.Cookie.MaxAge = o.ExpireTimeSpan;
            o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            o.Cookie.SameSite = SameSiteMode.None;
        });
        services.AddAuthorization();
        services.AddScoped<LocalServerAuthenticationEvents>();
    }

    public static void UseLocalAuthentication(this WebApplication app, IEndpointRouteBuilder? route = null)
    {

        app.UseAuthentication();
        app.UseAuthorization();
        LocalAuthenticationEndpoints.Map(route ?? app);
    }
}