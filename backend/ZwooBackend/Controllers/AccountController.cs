using BackendHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;
using ZwooBackend.Services;
using ZwooDatabase;

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")]
[Route("account")]
public class AccountController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILanguageService _languageService;
    private readonly IUserService _userService;
    private ICaptchaService _captcha;


    public AccountController(IEmailService emailService, ILanguageService languageService, IUserService userService, ICaptchaService captcha)
    {
        _emailService = emailService;
        _languageService = languageService;
        _userService = userService;
        _captcha = captcha;
    }

    [HttpGet("settings")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public IActionResult GetSettings()
    {
        var cookie = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        var activeSession = _userService.IsUserLoggedIn(cookie.UserId, cookie.SessionId);
        if (activeSession.User == null || activeSession.SessionId == null || activeSession.Error != null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(activeSession.Error), "user not logged in"));
        }

        return Ok(new UserSettings() { Settings = activeSession.User.Settings });
    }

    [HttpPost("settings")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public IActionResult PostSettings([FromBody] SetSettings body)
    {
        var cookie = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        var activeSession = _userService.IsUserLoggedIn(cookie.UserId, cookie.SessionId);
        if (activeSession.User == null || activeSession.SessionId == null || activeSession.Error != null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(activeSession.Error), "user not logged in"));
        }

        activeSession.User.Settings = body.settings;
        _userService.UpdateUser(activeSession.User, AuditOptions.WithActor(AuditActor.User(activeSession.User.Id, activeSession.SessionId)).AddMessage("updated settings"));
        return Ok(new MessageDTO() { Message = "updated settings" });
    }

    [HttpPost("changePassword")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public IActionResult ChangePassword([FromBody] ChangePassword body)
    {
        if (!StringHelper.IsValidPassword(body.newPassword))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "New Password Invalid!"));
        }

        var cookie = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        var activeSession = _userService.IsUserLoggedIn(cookie.UserId, cookie.SessionId);
        if (activeSession.User == null || activeSession.SessionId == null || activeSession.Error != null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(activeSession.Error), "user not logged in"));
        }

        if (!_userService.ChangePassword(activeSession.User, body.oldPassword, body.newPassword, activeSession.SessionId))
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.PASSWORD_NOT_MATCHING, "Password did not match"));
        }
        return Ok(new MessageDTO() { Message = "Password changed" });
    }

    [HttpPost("requestPasswordReset")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDTO))]
    public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetPassword body)
    {
        var captchaResponse = await _captcha.Verify(body.captchaToken);
        if (captchaResponse == null || !captchaResponse.Success)
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.CAPTCHA_INVALID, "Operation needs valid captcha token"));
        }

        var user = _userService.RequestChangePassword(body.email);
        if (user == null)
        {
            return NotFound(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "no user found"));
        }

        var recipient = _emailService.CreateRecipient(user.Email, user.Username, _languageService.ResolveFormQuery(HttpContext.Request.Query["lng"].FirstOrDefault() ?? ""));
        _emailService.SendPasswordResetMail(recipient, user.PasswordResetCode ?? "");
        return Ok(new MessageDTO() { Message = "password change email sent" });
    }

    [HttpPost("resetPassword")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDTO))]
    public IActionResult ResetPassword([FromBody] ResetPassword body)
    {
        if (!StringHelper.IsValidPassword(body.password))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));
        }

        var user = _userService.ResetPassword(body.code, body.password);
        if (user == null)
        {
            return NotFound(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "no user with this reset code found"));
        }
        return Ok(new MessageDTO() { Message = "reset user password" });
    }
}