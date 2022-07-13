using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BackendHelper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;

namespace ZwooBackend.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : Controller
{
    [HttpPost("recaptcha")]
    [Consumes(MediaTypeNames.Text.Plain)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult reCaptcha()
    {
        Globals.Logger.Info("POST /recaptch");
        HttpClient client = new HttpClient();
       
        using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
        {  
            var s = reader.ReadToEndAsync();
            s.Wait();
            var res = client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={Environment.GetEnvironmentVariable("ZWOO_RECAPTCHA_SIDESECRET")}&response={s.Result}", null);
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
        if (Globals.ZwooDatabase.EntryExists("username", body.username))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.USERNAME_ALREADY_TAKEN,
                "Username already Exists!"));
        if (Globals.ZwooDatabase.EntryExists("email", body.email))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.EMAIL_ALREADY_TAKEN,
                "Email already Exists!"));
        
        if (Globals.IsBeta)
            if (!Globals.ZwooDatabase.UseBetaCode(body.code))
                return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_BETACODE,
                    "Invalid or Missing Beta-code!"));
        
        var data = Globals.ZwooDatabase.CreateUser(body.username, body.email, body.password);
        
        Globals.EmailQueue.Enqueue(new EmailData(data.Item1, data.Item2,data.Item3, data.Item4));
        
        Globals.Logger.Info($"Account with username {body.username} created");
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
        
        if (Globals.ZwooDatabase.LoginUser(body.email, body.password, out var sid, out var id))
        {
            var claims = new List<Claim>
            {
                new Claim("auth", Convert.ToBase64String(CryptoHelper.Encrypt(Encoding.UTF8.GetBytes($"{id},{sid}"))))
            };
            Globals.Logger.Info($"{id},{sid}");
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var t = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
            t.Wait();
            return Ok("user logged in!");
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.USER_NOT_FOUND, "could not log in"));
    }
    
    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetUser()
    {
        var auth =  HttpContext.User.FindFirst("auth");
        if (auth == null)
            return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.COOKIE_MISSING,
                "Missing Cookie"));

        if (Globals.ZwooDatabase.GetUser(
                Encoding.UTF8.GetString(CryptoHelper.Encrypt(Convert.FromBase64String(auth.Value))), out var user))
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
        var auth =  HttpContext.User.FindFirst("auth");
        if (auth == null)
            return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.COOKIE_MISSING,
                "Missing Cookie"));

        if (Globals.ZwooDatabase.GetUser(
                Encoding.UTF8.GetString(CryptoHelper.Encrypt(Convert.FromBase64String(auth.Value))), out var user))
        {
            Globals.ZwooDatabase.LogoutUser(user);
            var t = HttpContext.SignOutAsync();
            t.Wait();
            return Ok(JsonSerializer.Serialize(user));
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING,
            "Session ID not Matching"));
    }

    [HttpGet("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Delete([FromBody] string password)
    {
        if (!StringHelper.IsValidPassword(password))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));
        
        var auth =  HttpContext.User.FindFirst("auth");
        if (auth == null)
            return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.COOKIE_MISSING,
                "Missing Cookie"));

        if (Globals.ZwooDatabase.GetUser(
                Encoding.UTF8.GetString(CryptoHelper.Encrypt(Convert.FromBase64String(auth.Value))), out var user))
        {
            if (Globals.ZwooDatabase.DeleteUser(user, password))
                return Ok("Account Deleted");
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING,
            "Session ID not Matching"));
    }
}