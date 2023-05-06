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

    public AuthenticationController(IEmailService emailService, ILanguageService languageService, IUserService userService, IBetaCodesService betaCodes)
    {
        _emailService = emailService;
        _languageService = languageService;
        _userService = userService;
        _betaCodes = betaCodes;
    }

    [HttpPost("recaptcha")]
    [Consumes(MediaTypeNames.Text.Plain)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult reCaptcha()
    {
        HttpClient client = new HttpClient();

        using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            var s = reader.ReadToEndAsync();
            s.Wait();
            var res = client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={Globals.RecaptchaSideSecret}&response={s.Result}", null);
            res.Wait();
            var body = res.Result.Content.ReadAsStringAsync();
            body.Wait();
            return Ok(body.Result);
        }
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDTO))]
    public IActionResult CreateAccount([FromBody] CreateAccount body)
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
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDTO))]
    public IActionResult Login([FromBody] Login body)
    {
        if (!StringHelper.IsValidEmail(body.email))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_EMAIL, "Email Invalid!"));
        }
        if (!StringHelper.IsValidPassword(body.password))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));
        }

        var result = _userService.LoginUser(body.email, body.password);
        if (result.User != null && result.SessionId != null)
        {
            var claims = new List<Claim>
            {
                new Claim("auth", CookieHelper.CreateCookieValue(result.User.Id, result.SessionId)),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)).Wait();
            return Ok(new MessageDTO() { Message = "user logged in!" });
        }
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(result.Error), "could not log in"));
    }

    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDTO))]
    public IActionResult GetUser()
    {
        var data = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        if (data.UserId == "")
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "user not found"));
        }

        var user = _userService.GetUserById(Convert.ToUInt64(data.UserId));
        if (user == null)
        {
            return NotFound(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "user not found"));
        }

        if (user.Sid.Contains(data.SessionId))
        {
            return Ok(user.ToDTO());
        }
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Session ID not Matching"));
    }

    [HttpGet("logout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDTO))]
    public IActionResult Logout()
    {
        var data = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        if (data.UserId == "")
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "user not found"));
        }

        var user = _userService.GetUserById(Convert.ToUInt64(data.UserId));
        if (user == null)
        {
            return NotFound(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "user not found"));
        }

        if (_userService.LogoutUser(user, data.SessionId))
        {
            HttpContext.SignOutAsync().Wait();
            return Ok(new MessageDTO() { Message = "logged out" });
        }
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Session ID not Matching"));
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDTO))]
    public IActionResult Delete([FromBody] Delete body)
    {
        if (!StringHelper.IsValidPassword(body.password))
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));

        var data = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        if (data.UserId == "")
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "user not found"));
        }

        var user = _userService.GetUserById(Convert.ToUInt64(data.UserId));
        if (user == null)
        {
            return NotFound(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "user not found"));
        }

        if (_userService.DeleteUser(user, body.password, AuditOptions.WithActor(AuditActor.User(user.Id, data.SessionId))))
        {
            HttpContext.SignOutAsync().Wait();
            return Ok(new MessageDTO() { Message = "account deleted out" });
        }
        return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Session ID not Matching"));
    }

    [HttpPost("resendVerificationEmail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDTO))]
    public IActionResult ResendVerificationEmail([FromBody] VerificationEmail body)
    {
        if (!StringHelper.IsValidEmail(body.email))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_EMAIL, "Email Invalid!"));
        }
        if (!_userService.ExistsEmail(body.email))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.USER_NOT_FOUND, "User does not exist!"));
        }

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