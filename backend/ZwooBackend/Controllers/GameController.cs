using System.Text;
using BackendHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;

namespace ZwooBackend.Controllers;

[ApiController]
[EnableCors("Zwoo")] 
[Route("game")]
public class GameController : Controller
{
    [HttpGet("leaderboard")]
    public LeaderBoard GetLeaderBoard()
    {
        return Globals.ZwooDatabase.GetLeaderBoard();
    }
    
    [HttpGet("leaderboard/position")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetLeaderBoardPosition()
    {
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user))
            return Ok($"{{\"position\": {Globals.ZwooDatabase.GetPosition(user)}}}");
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.COOKIE_MISSING,
            "Missing Cookie"));
    }
}