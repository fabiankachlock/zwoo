using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BackendHelper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;
using ZwooBackend.Services;
using ZwooDatabase;

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")]
[Route("auth")]
public class AuthenticationController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILanguageService _languageService;
    private readonly IUserService _userService;
    private readonly IBetaCodesService _betaCodes;
    private ICaptchaService _captcha;


    public AuthenticationController(IEmailService emailService, ILanguageService languageService, IUserService userService, IBetaCodesService betaCodes, ICaptchaService captcha)
    {
        _emailService = emailService;
        _languageService = languageService;
        _userService = userService;
        _betaCodes = betaCodes;
        _captcha = captcha;
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDTO))]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccount body)
    {
        if (!StringHelper.IsValidEmail(body.email))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_EMAIL, "Email Invalid!"));
        }
        if (!StringHelper.IsValidUsername(body.username))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_USERNAME, "Username Invalid!"));
        }
        if (!StringHelper.IsValidPassword(body.password))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));
        }
        if (_userService.ExistsUsername(body.username))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.USERNAME_ALREADY_TAKEN, "Username already Exists!"));
        }
        if (_userService.ExistsEmail(body.email))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.EMAIL_ALREADY_TAKEN, "Email already Exists!"));
        }

        if (Globals.IsBeta)
        {
            if (body.code == null || !_betaCodes.ExistsBetaCode(body.code))
            {
                return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_BETACODE, "Invalid or Missing Beta-code!"));
            }
        }

        var captchaResponse = await _captcha.Verify(body.captchaToken);
        if (captchaResponse == null || !captchaResponse.Success)
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.CAPTCHA_INVALID, "Operation needs valid captcha token"));
        }

        var user = _userService.CreateUser(body.username, body.email, body.password, body.code);
        var recipient = _emailService.CreateRecipient(body.email, body.username, _languageService.ResolveFormQuery(HttpContext.Request.Query["lng"].FirstOrDefault() ?? ""));
        _emailService.SendVerifyMail(recipient, user.Id, user.ValidationCode);
        return Ok(new MessageDTO() { Message = "account created" });
    }

    [HttpGet("verify")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDTO))]
    public IActionResult VerifyAccount([FromQuery(Name = "id")] UInt64 id, [FromQuery(Name = "code")] string code)
    {
        if (_userService.VerifyUser(id, code, Globals.IsBeta) != null)
        {
            return Ok(new MessageDTO() { Message = "account verified" });
        }

        return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.ACCOUNT_FAILED_TO_VERIFIED, "could not verify the account!"));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public async Task<IActionResult> Login([FromBody] Login body)
    {
        var captchaResponse = await _captcha.Verify(body.captchaToken);
        if (captchaResponse == null || !captchaResponse.Success)
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.CAPTCHA_INVALID, "Operation needs valid captcha token"));
        }

        var result = _userService.LoginUser(body.email, body.password);
        if (result.User == null || result.SessionId == null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(result.Error), "could not log in"));
        }

        var claims = new List<Claim>
        {
            new Claim("auth", CookieHelper.CreateCookieValue(result.User.Id, result.SessionId)),
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)).Wait();
        return Ok(new MessageDTO() { Message = "user logged in!" });
    }

    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public IActionResult GetUser()
    {
        var (user, sessionId) = CookieHelper.GetUser(_userService, HttpContext.User.FindFirst("auth")?.Value);
        if (user == null || sessionId == null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "user not found"));
        }

        if (user.Sid.Contains(sessionId))
        {
            return Ok(user.ToDTO());
        }
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Session ID not Matching"));
    }

    [HttpGet("logout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public IActionResult Logout()
    {
        var (user, sessionId) = CookieHelper.GetUser(_userService, HttpContext.User.FindFirst("auth")?.Value);
        if (user == null || sessionId == null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "session id not found"));
        }

        if (_userService.LogoutUser(user, sessionId))
        {
            HttpContext.SignOutAsync().Wait();
            return Ok(new MessageDTO() { Message = "logged out" });
        }
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "cant logout session"));
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public IActionResult Delete([FromBody] Delete body)
    {
        var (user, sessionId) = CookieHelper.GetUser(_userService, HttpContext.User.FindFirst("auth")?.Value);
        if (user == null || sessionId == null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "session id not found"));
        }

        if (_userService.DeleteUser(user, body.password, AuditOptions.WithActor(AuditActor.User(user.Id, sessionId))))
        {
            HttpContext.SignOutAsync().Wait();
            return Ok(new MessageDTO() { Message = "account deleted out" });
        }
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.PASSWORD_NOT_MATCHING, "cant delete account"));
    }

    [HttpPost("resendVerificationEmail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDTO))]
    public IActionResult ResendVerificationEmail([FromBody] VerificationEmail body)
    {
        var user = _userService.GetUserByEmail(body.email);
        if (user == null)
        {
            return NotFound(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "user not found"));
        }

        if (user.Verified)
        {
            return BadRequest(new ErrorDTO() { Message = "already verified" });
        }

        user.ValidationCode = StringHelper.GenerateNDigitString(6);
        _userService.UpdateUser(user, AuditOptions.WithMessage("new verification email sent"));

        var recipient = _emailService.CreateRecipient(user.Email, user.Username, _languageService.ResolveFormQuery(HttpContext.Request.Query["lng"].FirstOrDefault() ?? ""));
        _emailService.SendVerifyMail(recipient, user.Id, user.ValidationCode);
        return Ok(new MessageDTO() { Message = "Email resend!" });
    }
}