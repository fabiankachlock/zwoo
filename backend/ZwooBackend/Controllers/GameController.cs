using System.Text;
using BackendHelper;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;

namespace ZwooBackend.Controllers;

[ApiController]
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
        var auth =  HttpContext.User.FindFirst("auth");
        if (auth == null)
            return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.COOKIE_MISSING,
                "Missing Cookie"));
        return Ok($"{{\"position\": {Globals.ZwooDatabase.GetPosition(Encoding.UTF8.GetString(CryptoHelper.Encrypt(Convert.FromBase64String(auth.Value))))}}}");
    }
}