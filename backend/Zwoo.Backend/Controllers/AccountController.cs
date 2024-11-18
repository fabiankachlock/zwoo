using BackendHelper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Zwoo.Backend.Controllers.DTO;
using Zwoo.Backend.Shared.Api.Model;
using Zwoo.Backend.Shared.Authentication;
using Zwoo.Backend.Shared.Configuration;
using Zwoo.Backend.Shared.Services;
using Zwoo.Database;

namespace Zwoo.Backend.Controllers;

[ApiController]
[Route("account")]
public class AccountController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILanguageService _languageService;
    private readonly IUserService _userService;
    private readonly IBetaCodesService _betaCodes;
    private readonly ICaptchaService _captcha;
    private readonly bool _isBeta;

    public AccountController(IEmailService emailService, ILanguageService languageService, IUserService userService, ICaptchaService captcha, IBetaCodesService betaCodesService, IOptions<ZwooOptions> options)
    {
        _emailService = emailService;
        _languageService = languageService;
        _userService = userService;
        _captcha = captcha;
        _betaCodes = betaCodesService;
        _isBeta = options.Value.Features.IsBeta;
    }

    [AllowAnonymous]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccount body)
    {
        if (!body.AcceptedTerms)
        {
            return this.BadRequest(ApiError.InvalidTerms, "Accept terms", "Needs terms to be accepted.");
        }
        if (!StringHelper.IsValidEmail(body.Email))
        {
            return this.BadRequest(ApiError.InvalidEmail, "Invalid email", "The email address is in an invalid format.");
        }
        if (!StringHelper.IsValidUsername(body.Username))
        {
            return this.BadRequest(ApiError.InvalidUsername, "Invalid username", "The username is in an invalid format.");
        }
        if (!StringHelper.IsValidPassword(body.Password))
        {
            return this.BadRequest(ApiError.InvalidPassword, "Invalid password", "The password is in an invalid format.");
        }
        if (_userService.ExistsUsername(body.Username))
        {
            return this.BadRequest(ApiError.UsernameTaken, "Username is taken", "The username specified is already taken by another user.");
        }
        if (_userService.ExistsEmail(body.Email))
        {
            return this.BadRequest(ApiError.EmailTaken, "Email is taken", "The email specified is already associated with an account.");
        }

        if (_isBeta)
        {
            if (body.Code == null || !_betaCodes.ExistsBetaCode(body.Code))
            {
                return this.BadRequest(ApiError.InvalidBetaCode, "Invalid BetaCode", "The beta code specified does not exist or is already redeemed.");
            }
        }

        var captchaResponse = await _captcha.Verify(body.CaptchaToken);
        if (captchaResponse == null || !captchaResponse.Success)
        {
            return this.BadRequest(ApiError.InvalidCaptcha, "Invalid captcha token", "The captcha cannot be verified.");
        }

        var user = _userService.CreateUser(body.Username, body.Email, body.Password, body.AcceptedTerms, body.Code);
        var recipient = _emailService.CreateRecipient(body.Email, body.Username, _languageService.ResolveFormQuery(HttpContext.Request.Query["lng"].FirstOrDefault() ?? ""));
        _emailService.SendVerifyMail(recipient, user.Id, user.ValidationCode);
        return Ok();
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteAccount body)
    {

        var activeSession = HttpContext.GetActiveUser();
        if (_userService.DeleteUser(activeSession.User, body.Password, AuditOptions.WithActor(AuditActor.User(activeSession.User.Id, activeSession.ActiveSession))))
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }
        return this.Unauthorized(ApiError.PasswordMismatch, "Cant delete account", "The passwords did not match.");
    }

    [AllowAnonymous]
    [HttpGet("verify")]
    public IActionResult VerifyAccount([FromQuery(Name = "id")] ulong id, [FromQuery(Name = "code")] string code)
    {
        if (_userService.VerifyUser(id, code, _isBeta) != null)
        {
            return Ok();
        }
        return this.BadRequest(ApiError.VerifyFailed, "Cant verify account", "The account could not be verified.");
    }

    [AllowAnonymous]
    [HttpPost("verify/resend")]
    public IActionResult ResendVerificationEmail([FromBody] VerifyEmail body)
    {
        var user = _userService.GetUserByEmail(body.Email);
        if (user == null)
        {
            return this.NotFound(ApiError.UserNotFound, "User not found", "A user with this email address cannot be found.");
        }

        if (user.Verified)
        {
            return this.BadRequest(ApiError.AlreadyVerified, "User verified", "The user is already verified");
        }

        user.ValidationCode = StringHelper.GenerateNDigitString(6);
        _userService.UpdateUser(user, AuditOptions.WithMessage("new verification email sent"));

        var recipient = _emailService.CreateRecipient(user.Email, user.Username, _languageService.ResolveFormQuery(HttpContext.Request.Query["lng"].FirstOrDefault() ?? ""));
        _emailService.SendVerifyMail(recipient, user.Id, user.ValidationCode);
        return Ok();
    }

    [HttpGet("settings")]
    public IActionResult GetSettings()
    {
        var activeSession = HttpContext.GetActiveUser();
        return Ok(new UserSettings() { Settings = activeSession.User.Settings });
    }

    [HttpPost("settings")]
    public IActionResult PostSettings([FromBody] UserSettings body)
    {
        var activeSession = HttpContext.GetActiveUser();
        activeSession.User.Settings = body.Settings;
        _userService.UpdateUser(activeSession.User, AuditOptions.WithActor(AuditActor.User(activeSession.User.Id, activeSession.ActiveSession)).AddMessage("updated settings"));
        return Ok();
    }

    [HttpPost("password/change")]
    public IActionResult ChangePassword([FromBody] ChangePassword body)
    {
        if (!StringHelper.IsValidPassword(body.NewPassword))
        {
            return this.BadRequest(ApiError.InvalidPassword, "Invalid password", "The password is in an invalid format.");
        }

        var activeSession = HttpContext.GetActiveUser();
        if (!_userService.ChangePassword(activeSession.User, body.OldPassword, body.NewPassword, activeSession.ActiveSession))
        {
            return this.Unauthorized(ApiError.PasswordMismatch, "Cant change password", "The passwords did not match.");
        }
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("password/request-reset")]
    public async Task<IActionResult> RequestResetPassword([FromBody] RequestResetPassword body)
    {
        var captchaResponse = await _captcha.Verify(body.CaptchaToken);
        if (captchaResponse == null || !captchaResponse.Success)
        {
            return this.BadRequest(ApiError.InvalidCaptcha, "Invalid captcha token", "The captcha cannot be verified.");
        }

        var user = _userService.RequestChangePassword(body.Email);
        if (user == null)
        {
            return this.NotFound(ApiError.UserNotFound, "User not found", "A user with this email address cannot be found.");
        }

        var recipient = _emailService.CreateRecipient(user.Email, user.Username, _languageService.ResolveFormQuery(HttpContext.Request.Query["lng"].FirstOrDefault() ?? ""));
        _emailService.SendPasswordResetMail(recipient, user.PasswordResetCode ?? "");
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("password/reset")]
    public IActionResult ResetPassword([FromBody] ResetPassword body)
    {
        if (!StringHelper.IsValidPassword(body.Password))
        {
            return this.BadRequest(ApiError.InvalidPassword, "Invalid password", "The password is in an invalid format.");
        }

        var user = _userService.ResetPassword(body.Code, body.Password);
        if (user == null)
        {
            return this.NotFound(ApiError.UserNotFound, "User not found", "A user with this password reset code cannot be found.");
        }
        return Ok();
    }
}