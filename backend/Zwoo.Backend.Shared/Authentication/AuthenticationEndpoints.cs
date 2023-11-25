using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Services;
using Zwoo.Database;

namespace Zwoo.Backend.Shared.Authentication;

public static class AuthenticationEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/auth/login", async ([FromBody] Login body, HttpContext context, ICaptchaService _captchaService, IUserService _userService) =>
        {
            var captchaResponse = await _captchaService.Verify(body.CaptchaToken);
            if (captchaResponse == null || !captchaResponse.Success)
            {
                return Results.BadRequest(ApiError.InvalidCaptcha.ToProblem(new ProblemDetails()
                {
                    Title = "Invalid captcha token",
                    Detail = "The captcha cannot be verified.",
                    Instance = context.Request.Path
                }));
            }
            var result = _userService.LoginUser(body.Email, body.Password);
            if (result.Error != null || result.User == null || result.SessionId == null)
            {
                return Results.Problem(result.Error.ToApi().ToProblem(new ProblemDetails()
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Login failed",
                    Detail = "The user cant be logged in with these credentials.",
                    Instance = context.Request.Path,
                }));
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

            return Results.Problem(ApiError.SessionIdMismatch.ToProblem(new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Cant sign out session",
                Detail = "Sign out operation failed unexpectedly.",
            }));
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