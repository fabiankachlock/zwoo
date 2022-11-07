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

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")] 
[Route("auth")]
public class AuthenticationController : Controller
{
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateAccount([FromBody] CreateAccount body)
    {
        if (!StringHelper.IsValidEmail(body.email))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_EMAIL, "Email Invalid!"));
        if (!StringHelper.IsValidUsername(body.username))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_USERNAME, "Username Invalid!"));
        if (!StringHelper.IsValidPassword(body.password))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));
        if (Globals.ZwooDatabase.UsernameExists(body.username))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.USERNAME_ALREADY_TAKEN,
                "Username already Exists!"));
        if (Globals.ZwooDatabase.EmailExists(body.email))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.EMAIL_ALREADY_TAKEN,
                "Email already Exists!"));
        
        if (Globals.IsBeta)
            if (!Globals.ZwooDatabase.CheckBetaCode(body.code))
                return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_BETACODE,
                    "Invalid or Missing Beta-code!"));
        
        var data = Globals.ZwooDatabase.CreateUser(body.username, body.email, body.password, body.code);
        
        Globals.EmailQueue.Enqueue(new EmailData(data.Item1, data.Item2,data.Item3, data.Item4));        
        return Ok("{\"message\": \"Account create\"}");
    }

    [HttpGet("verify")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult VerifyAccount([FromQuery(Name = "id")] UInt64 id, [FromQuery(Name = "code")] string code)
    {
        if (Globals.ZwooDatabase.VerifyUser(id, code))
            return Ok("{\"message\": \"Account verified\"}");
        else
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.ACCOUNT_FAILED_TO_VERIFIED,
                "could not verify the account!"));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Login([FromBody] Login body)
    {
        if (!StringHelper.IsValidEmail(body.email))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_EMAIL, "Email Invalid!"));
        if (!StringHelper.IsValidPassword(body.password))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));
        
        if (Globals.ZwooDatabase.LoginUser(body.email, body.password, out var sid, out var id, out var error))
        {
            var claims = new List<Claim>
            {
                new Claim("auth", $"{id},{sid}")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var t = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            t.Wait();
            return Ok("user logged in!");
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(error, "could not log in"));
    }
    
    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetUser()
    {
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user, out _))
        {
            return Ok(JsonSerializer.Serialize(user));
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING,
            "Session ID not Matching"));
    }

    [HttpGet("logout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user, out var sid))
        {
            Globals.ZwooDatabase.LogoutUser(user, sid);
            var t = HttpContext.SignOutAsync();
            t.Wait();
            return Ok(JsonSerializer.Serialize(user));
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING,
            "Session ID not Matching"));
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Delete([FromBody] Delete body)
    {
        if (!StringHelper.IsValidPassword(body.password))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));
        
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user, out _))
        {
            if (Globals.ZwooDatabase.DeleteUser(user, body.password))
            {
                var t = HttpContext.SignOutAsync();
                t.Wait();
                return Ok("Account Deleted");
            }
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING,
            "Session ID not Matching"));
    }
    
    [HttpPost("resendVerificationEmail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ResendVerificationEmail([FromBody] VerificationEmail body)
    {
        if (!StringHelper.IsValidEmail(body.email))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_EMAIL, "Email Invalid!"));
        if (!Globals.ZwooDatabase.EmailExists(body.email))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.USER_NOT_FOUND, "User does not exist!"));
        
        var u = Globals.ZwooDatabase.GetUserFromEmail(body.email)!;
        if (u.Verified) return BadRequest("already verified");
        u.ValidationCode = StringHelper.GenerateNDigitString(6);
        Globals.ZwooDatabase.UpdateUser(u);
        
        Globals.EmailQueue.Enqueue(new EmailData(u.Username, u.Id, u.ValidationCode, u.Email));
        return Ok("Email resend!");
    }
}