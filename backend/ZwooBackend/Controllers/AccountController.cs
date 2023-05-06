using BackendHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;
using ZwooBackend.Services;

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")]
[Route("account")]
public class AccountController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILanguageService _languageService;

    public AccountController(IEmailService emailService, ILanguageService languageService)
    {
        _emailService = emailService;
        _languageService = languageService;
    }

    [HttpGet("settings")]
    public IActionResult GetSettings()
    {
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user, out var sid))
            return Ok($"{{\"settings\": \"{user.Settings.Replace("\"", "\\\"")}\"}}");
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Session ID not Matching"));
    }

    [HttpPost("settings")]
    public IActionResult PostSettings([FromBody] SetSettings body)
    {
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user, out var sid))
        {
            user.Settings = body.settings;
            Globals.ZwooDatabase.UpdateUser(user);
            return Ok("");
        }
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Session ID not Matching"));
    }

    [HttpPost("changePassword")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ChangePassword([FromBody] ChangePassword body)
    {
        if (!StringHelper.IsValidPassword(body.oldPassword))
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "Old Password Invalid!"));
        if (!StringHelper.IsValidPassword(body.newPassword))
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "New Password Invalid!"));

        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user, out var sid))
        {
            if (Globals.ZwooDatabase.ChangePassword(user, body.oldPassword, body.newPassword, sid))
                return Ok("Password changed");
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.PASSWORD_NOT_MATCHING, "Password did not match"));
        }
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Session ID not Matching"));
    }

    [HttpPost("requestPasswordReset")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult RequestResetPassword([FromBody] RequestResetPassword body)
    {
        if (!StringHelper.IsValidEmail(body.email))
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_EMAIL, "Email Invalid!"));
        if (!Globals.ZwooDatabase.EmailExists(body.email))
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "no User with this Email"));

        var user = Globals.ZwooDatabase.RequestChangePassword(body.email);
        var recipient = _emailService.CreateRecipient(user.Email, user.Username, _languageService.ResolveFormQuery(HttpContext.Request.Query["lng"].FirstOrDefault() ?? ""));
        _emailService.SendPasswordResetMail(recipient, user.PasswordResetCode ?? "");
        return Ok("");
    }

    [HttpPost("resetPassword")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ResetPassword([FromBody] ResetPassword body)
    {
        if (!StringHelper.IsValidPassword(body.password))
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));
        Globals.ZwooDatabase.ResetPassword(body.code, body.password);
        return Ok("");
    }
}