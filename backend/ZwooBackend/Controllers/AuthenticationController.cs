using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using BackendHelper;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;

namespace ZwooBackend.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("auth")]
public class AuthenticationController : Controller
{
    [Microsoft.AspNetCore.Mvc.HttpPost("recaptcha")]
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
    
    [Microsoft.AspNetCore.Mvc.HttpPost("create")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateAccount([Microsoft.AspNetCore.Mvc.FromBody] CreateAccount body)
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

    [Microsoft.AspNetCore.Mvc.HttpGet("verify")]
    [Consumes(MediaTypeNames.Application.Json)]
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
    
    [Microsoft.AspNetCore.Mvc.HttpGet("user")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetUser()
    {
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING,
            "Session ID not Matching"));
    }
}