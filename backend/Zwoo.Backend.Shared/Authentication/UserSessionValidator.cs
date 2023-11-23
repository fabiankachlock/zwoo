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
                // save the current data to the http context for use in route handlers
                context.HttpContext.Items.Add(HttpContextExtensions.UserContextKey, loginResult.User);
                context.HttpContext.Items.Add(HttpContextExtensions.SessionIdContextKey, loginResult.SessionId);
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
        // TODO: send full details response
        await context.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Detail = "You need to be logged in"
        });
    }
}