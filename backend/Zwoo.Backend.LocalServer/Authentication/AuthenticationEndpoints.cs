using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.LocalServer.Services;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Authentication;


namespace Zwoo.Backend.LocalServer.Authentication;

public class LocalAuthenticationEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/auth/login-guest", async ([FromBody] GuestLogin body, HttpContext context, ILocalUserManager _userService) =>
        {
            var result = _userService.CreateGuest(body.Username);
            if (result.Error != null || result.User == null)
            {
                return Results.Problem((result.Error ?? ApiError.None).ToProblem(new ProblemDetails()
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Login failed",
                    Detail = "The user cant be logged in with these credentials.",
                    Instance = context.Request.Path,
                }));
            }

            var sessionData = new GuestSessionData(result.User.Id);
            var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            await context.SignInAsync(scheme, sessionData.ToPrincipal(scheme));
            return Results.Ok();
        }).AllowAnonymous();

        app.MapGet("/auth/logout", async (HttpContext context, ILocalUserManager _userService) =>
        {
            var user = context.GetActiveGuest();
            if (_userService.DeleteGuest(user.User.Id))
            {
                await context.SignOutAsync();
                return Results.Ok();
            }

            return Results.Problem(ApiError.SessionIdMismatch.ToProblem(new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Cant sign out session",
                Detail = "Sign out operation failed unexpectedly.",
            }));
        });

        app.MapGet("/auth/user", (HttpContext context) =>
        {
            var user = context.GetActiveGuest();
            return Results.Ok(new UserSession()
            {
                Email = user.Name,
                Username = user.Name,
                Wins = 0
            });
        });
    }
}