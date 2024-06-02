using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Controllers.DTO;
using Zwoo.Backend.Shared.Authentication;
using Zwoo.Database;

namespace Zwoo.Backend.Controllers;

[ApiController]
[Route("leaderboard")]
public class LeaderboardController : Controller
{
    private IGameInfoService _gameInfo;

    public LeaderboardController(IGameInfoService gameInfo)
    {
        _gameInfo = gameInfo;
    }

    [AllowAnonymous]
    [HttpGet("")]
    public IActionResult GetLeaderBoard()
    {
        return Ok(_gameInfo.GetLeaderBoard().ToDTO());
    }

    [HttpGet("self")]
    public IActionResult GetLeaderBoardPosition()
    {
        var activeSession = HttpContext.GetActiveUser();
        return Ok(new LeaderBoardPosition()
        {
            Position = _gameInfo.GetPosition(activeSession.User)
        });
    }
}