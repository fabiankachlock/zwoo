namespace Zwoo.Backend.Shared.Authentication;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Database;

public class ZwooCookieAuthenticationEvents : CookieAuthenticationEvents
{
    private readonly IUserService _userService;

    public ZwooCookieAuthenticationEvents(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userPrincipal = context.Principal;
        var session = UserSessionData.FromClaims(context.Principal?.Claims ?? []);

        if (session != null)
        {
            var loginResult = _userService.IsUserLoggedIn(session.UserId, session.SessionId);
            if (loginResult.User != null && loginResult.SessionId != null && loginResult.Error == null)
            {
                context.HttpContext.StoreUserSession(loginResult.User, loginResult.SessionId);
                return;
            }
        }

        // default: sign out if session is invalid or expired
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public override async Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        // disable default redirect to login behavior
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Title = "Access unauthorized",
            Detail = "You need to be logged in",
            Status = StatusCodes.Status401Unauthorized,
            Instance = context.Request.Path
        });
    }
}