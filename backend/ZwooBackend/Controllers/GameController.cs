using ZwooBackend.ZRP;
using System.Text.Json;
using System.Text;
using BackendHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Controllers.DTO;
using ZwooBackend.Games;

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
                    return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.GAME_NAME_MISSING, "Insufficient create game data"));
                }

                long gameId = GameManager.Global.CreateGame(body.Name, !body.UsePassword.Value);
                GameManager.Global.GetGame(gameId)?.Lobby.Initialize((long)user.Id, user.Username, body.Password ?? "", body.UsePassword.Value);

                Globals.Logger.Info($"{user.Id} created game {gameId}");
                return Ok(JsonSerializer.Serialize(new JoinGameResponse(gameId, false, ZRPRole.Host)));
            }
            else if (body.Opcode == ZRPRole.Player || body.Opcode == ZRPRole.Spectator)
            {
                if (body.GameId == null)
                {
                    return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_GAMEID, "gameid is null"));
                }

                GameRecord? game = GameManager.Global.GetGame(body.GameId.Value);
                if (game == null)
                {
                    return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.GAME_NOT_FOUND, "no game found for id"));
                }

                if (game.Game.IsRunning && game.Lobby.GetPlayer(user.Username) == null)
                {
                    // join as spectator when game is already running and its an new player
                    // don't make the player a spectator when he his already in the game, because then hes rejoining
                    body.Opcode = ZRPRole.Spectator;
                }

                LobbyResult result = game.Lobby.AddPlayer((long)user.Id, user.Username, body.Opcode, body.Password ?? "");
                if (LobbyResult.Success != result)
                {
                    if (result == LobbyResult.ErrorLobbyFull)
                    {
                        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.GAME_FULL, "lobby is full"));
                    }
                    else if (result == LobbyResult.ErrorWrongPassword)
                    {
                        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_PASSWORD, "wrong password"));
                    } 
                    else

                    {
                        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.JOIN_FAILED, "cant join"));
                    }
                }

                var player = game.Lobby.GetPlayer((long)user.Id);
                Globals.Logger.Info($"{user.Id} joined game {game.Game.Id}");
                // the players opcode may changed based on rejoin
                return Ok(JsonSerializer.Serialize(new JoinGameResponse(game.Game.Id, game.Game.IsRunning, player == null ? body.Opcode : player.Role)));
            }

            return BadRequest(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_OPCODE, "invalid opcode"));
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Unauthorized"));
    }

    [HttpGet("games")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GamesListResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ListGames()
    {
        Globals.Logger.Info("GET /game/games");
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user))
        {
            List<GameRecord> games = GameManager.Global.ListAll();
            GamesListResponse response = new GamesListResponse(games.Select(game => new GameMetaResponse(game.Game.Id, game.Game.Name, game.Game.IsPublic, game.Lobby.PlayerCount())).ToArray());

            return Ok(JsonSerializer.Serialize(response));
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Unauthorized"));
    }

    [HttpGet("games/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameMetaResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetGames(long id)
    {
        Globals.Logger.Info($"GET /game/games/{id}");
        if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user))
        {
            GameRecord? game = GameManager.Global.GetGame(id);
            if (game == null)
            {
                return NotFound(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.GAME_NOT_FOUND, "game Not found"));
            }
            GameMetaResponse response = new GameMetaResponse(game.Game.Id, game.Game.Name, game.Game.IsPublic, game.Lobby.PlayerCount());

            return Ok(JsonSerializer.Serialize(response));
        }
        return Unauthorized(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Unauthorized"));
    }
}
