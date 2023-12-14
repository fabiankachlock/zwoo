using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.LocalServer.Services;
using Zwoo.Backend.Shared.Authentication;

namespace Zwoo.Backend.LocalServer.Authentication;

public class LocalServerAuthenticationEvents : CookieAuthenticationEvents
{
    private readonly ILocalUserManager _userService;

    public LocalServerAuthenticationEvents(ILocalUserManager userService)
    {
        _userService = userService;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var session = GuestSessionData.FromClaims(context.Principal?.Claims ?? []);

        if (session != null)
        {
            var result = _userService.IsGuestLoggedIn(session.UserId);
            if (result.User != null && result.Error == null)
            {
                context.HttpContext.StoreGuestSession(result.User);
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
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        await context.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Title = "Access unauthorized",
            Detail = "You need to be logged in",
            Status = StatusCodes.Status401Unauthorized,
            Instance = context.Request.Path
        });
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }
}