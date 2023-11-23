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
    public static void AddZwooAuthentication(this WebApplicationBuilder builder, ZwooOptions options)
    {
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
        {
            o.ExpireTimeSpan = TimeSpan.FromDays(90);
            // Dont use SlidingExpiration, because its an security issue!
            o.Cookie.Name = "auth";
            o.Cookie.HttpOnly = true;
            o.Cookie.MaxAge = o.ExpireTimeSpan;
            o.EventsType = typeof(ZwooCookieAuthenticationEvents);
            o.LoginPath = null;
            if (options.Server.UseSsl)
            {
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.SameSite = SameSiteMode.None;
            }
            o.Cookie.Domain = options.Server.CookieDomain;
        });
        builder.Services.AddAuthorization(o =>
        {
            o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });
        builder.Services.AddScoped<ZwooCookieAuthenticationEvents>();
    }

    public static void UseZwooAuthentication(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapPost("/auth/login", async ([FromBody] Login body, HttpContext context, IUserService _userService) =>
        {
            // TODO: captcha
            var result = _userService.LoginUser(body.Email, body.Password);
            if (result.User == null || result.SessionId == null)
            {
                return Results.Unauthorized();
            }

            var sessionData = new UserSessionData(result.User.Id, result.SessionId);
            var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            await context.SignInAsync(scheme, sessionData.ToPrincipal(scheme));
            return Results.Ok();
        }).AllowAnonymous();

        app.MapGet("/auth/logout", async (HttpContext context, IUserService _userService) =>
        {
            var user = context.GetActiveUser();
            if (_userService.LogoutUser(user.User, user.ActiveSession))
            {
                await context.SignOutAsync();
                return Results.Ok();
            }

            return Results.Problem(new ProblemDetails()
            {
                Detail = "sign out operation errored",
                Status = StatusCodes.Status500InternalServerError
            });
        });

        app.MapGet("/auth/user", (HttpContext context) =>
        {
            var user = context.GetActiveUser();
            return Results.Ok(new UserSession()
            {
                Email = user.Email,
                Username = user.Username,
                Wins = (int)user.Wins
            });
        });
    }
}