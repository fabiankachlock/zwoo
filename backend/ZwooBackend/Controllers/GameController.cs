using System.Text;
using BackendHelper;
using ZwooBackend.ZRP;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;
using ZwooBackend.Games;

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
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user))
            return Ok($"{{\"position\": {Globals.ZwooDatabase.GetPosition(user)}}}");
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.COOKIE_MISSING,
            "Missing Cookie"));
    }


    [HttpPost("join")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JoinGameResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult JoinGame([FromBody] JoinGame body)
    {

        Globals.Logger.Info("POST /game/join");
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user))
        {
            if (GameManager.Global.WebSocketManager.HasWebsocket((long)user.Id))
            {
                return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.ALREADY_INGAME, ""));
            }

            if (body.Opcode == ZRPRole.Host)
            {
                if (body.Name == null || body.UsePassword == null || (body.UsePassword == true && body.Password == null))
                {
                    return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.GAME_NAME_MISSING,
                                "Insufficient create game data"));
                }

                long gameId = GameManager.Global.CreateGame(body.Name, !body.UsePassword.Value);
                GameManager.Global.GetGame(gameId)?.Lobby.Initialize((long)user.Id, user.Username);

                Globals.Logger.Info($"{user.Id} created game {gameId}");
                return Ok(JsonSerializer.Serialize(new JoinGameResponse(gameId)));
            }
            else if (body.Opcode == ZRPRole.Player || body.Opcode == ZRPRole.Spectator)
            {
                if (body.GameId == null)
                {
                    return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_GAMEID, ""));
                }

                GameRecord? game = GameManager.Global.GetGame(body.GameId.Value);
                if (game == null)
                {
                    return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.GAME_NOT_FOUND, ""));
                }

                game.Lobby.AddPlayer((long)user.Id, user.Username, body.Opcode);
                Globals.Logger.Info($"{user.Id} joined game {game.Game.Id}");

                return Ok(JsonSerializer.Serialize(new JoinGameResponse(game.Game.Id)));
            }

            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_OPCODE, ""));
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Unauthorized"));
    }


}