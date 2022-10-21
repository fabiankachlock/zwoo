using BackendHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")] 
[Route("account")]
public class AccountController : Controller
{
    [HttpPost("changePassword")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ChangePassword([FromBody] ChangePassword body)
    {
        if (!StringHelper.IsValidPassword(body.oldPassword))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_PASSWORD, "Old Password Invalid!"));
        if (!StringHelper.IsValidPassword(body.newPassword))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_PASSWORD, "New Password Invalid!"));

        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user, out _))
        {
            if (Globals.ZwooDatabase.ChangePassword(user, body.oldPassword, body.newPassword))
                return Ok("Password changed");
            return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.PASSWORD_NOT_MATCHING, "Password did not match"));
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Session ID not Matching"));
    }
    
    [HttpPost("requestPasswordReset")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult RequestResetPassword([FromBody] RequestResetPassword body)
    {
        if (!StringHelper.IsValidEmail(body.email))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_EMAIL, "Email Invalid!"));
        if (!Globals.ZwooDatabase.EmailExists(body.email))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.USER_NOT_FOUND, "no User with this Email"));
        Globals.PasswordChangeRequestEmailQueue.Enqueue(Globals.ZwooDatabase.RequestChangePassword(body.email));
        return Ok("");
    }

    [HttpPost("resetPassword")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ResetPassword([FromBody] ResetPassword body)
    {
        if (!StringHelper.IsValidPassword(body.password))
            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_PASSWORD, "Password Invalid!"));
        Globals.ZwooDatabase.ResetPassword(body.code, body.password);
        return Ok("");
    }
}