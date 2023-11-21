using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Controllers.DTO;
using Zwoo.Backend.Websockets;
using Zwoo.Backend.Games;
using Zwoo.GameEngine;
using Zwoo.GameEngine.ZRP;
using Zwoo.GameEngine.Lobby;
using Zwoo.Database;

namespace Zwoo.Backend.Controllers;

[ApiController]
[EnableCors("Zwoo")]
[Route("game")]
public class GameController : Controller
{

    private IGameLogicService _gamesService;
    private IWebSocketManager _wsManager;
    private IGameInfoService _gameInfo;
    private IUserService _userService;

    public GameController(IGameLogicService gamesService, IWebSocketManager wsManager, IGameInfoService gameInfo, IUserService userService)
    {
        _gamesService = gamesService;
        _wsManager = wsManager;
        _gameInfo = gameInfo;
        _userService = userService;
    }

    [HttpGet("leaderboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LeaderBoard))]
    public IActionResult GetLeaderBoard()
    {
        return Ok(_gameInfo.GetLeaderBoard().ToDTO());
    }

    [HttpGet("leaderboard/position")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LeaderBoardPosition))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public IActionResult GetLeaderBoardPosition()
    {
        var cookie = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        var activeSession = _userService.IsUserLoggedIn(cookie.UserId, cookie.SessionId);
        if (activeSession.User == null || activeSession.SessionId == null || activeSession.Error != null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(activeSession.Error), "user not logged in"));
        }

        return Ok(new LeaderBoardPosition()
        {
            Position = _gameInfo.GetPosition(activeSession.User)
        });
    }


    [HttpPost("join")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JoinGameResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public IActionResult JoinGame([FromBody] JoinGame body)
    {
        var cookie = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        var activeSession = _userService.IsUserLoggedIn(cookie.UserId, cookie.SessionId);
        if (activeSession.User == null || activeSession.SessionId == null || activeSession.Error != null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(activeSession.Error), "user not logged in"));
        }


        if (_wsManager.HasConnection((long)activeSession.User.Id))
        {
            return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.ALREADY_INGAME, ""));
        }

        if (body.Opcode == ZRPRole.Host)
        {
            if (body.Name == null || body.UsePassword == null || (body.UsePassword == true && body.Password == null))
            {
                return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.GAME_NAME_MISSING, "Insufficient create game data"));
            }

            ZwooRoom game = _gamesService.CreateGame(body.Name, !body.UsePassword.Value);
            game.Lobby.Initialize((long)activeSession.User.Id, activeSession.User.Username, body.Password ?? "", body.UsePassword.Value);

            Globals.Logger.Info($"{activeSession.User.Id} created game {game.Id}");
            // TODO send dynamic lobby id
            return Ok(new JoinGameResponse(game.Id, false, ZRPRole.Host, 1));
        }
        else if (body.Opcode == ZRPRole.Player || body.Opcode == ZRPRole.Spectator)
        {
            if (body.GameId == null)
            {
                return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_GAMEID, "gameid is null"));
            }

            ZwooRoom? game = _gamesService.GetGame(body.GameId.Value);
            if (game == null)
            {
                return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.GAME_NOT_FOUND, "no game found for id"));
            }

            if (game.Game.IsRunning && game.Lobby.GetPlayerByUserName(activeSession.User.Username) == null)
            {
                // join as spectator when game is already running and its an new player
                // don't make the player a spectator when he his already in the game, because then hes rejoining
                body.Opcode = ZRPRole.Spectator;
            }

            LobbyResult result = game.Lobby.AddPlayer((long)activeSession.User.Id, game.NextId(), activeSession.User.Username, body.Opcode, body.Password ?? "");
            // TODO: move into ErrorCodes helper method like db error
            if (LobbyResult.Success != result)
            {
                if (result == LobbyResult.ErrorLobbyFull)
                {
                    return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.GAME_FULL, "lobby is full"));
                }
                else if (result == LobbyResult.ErrorWrongPassword)
                {
                    return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "wrong password"));
                }
                else
                {
                    return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.JOIN_FAILED, "cant join"));
                }
            }

            var player = game.Lobby.GetPossiblyPreparedPlayerByUserId((long)activeSession.User.Id);
            Globals.Logger.Info($"{activeSession.User.Id} joined game {game.Game.Id}");

            // the players opcode may changed based on rejoin
            return Ok(JsonSerializer.Serialize(new JoinGameResponse(game.Game.Id, game.Game.IsRunning, player == null ? body.Opcode : player.Role, player?.LobbyId ?? 0)));
        }

        return BadRequest(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_OPCODE, "invalid opcode"));
    }

    [HttpGet("games")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GamesListResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    public IActionResult ListGames()
    {
        var cookie = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        var activeSession = _userService.IsUserLoggedIn(cookie.UserId, cookie.SessionId);
        if (activeSession.User == null || activeSession.SessionId == null || activeSession.Error != null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(activeSession.Error), "user not logged in"));
        }

        IEnumerable<ZwooRoom> games = _gamesService.ListAll();
        GamesListResponse response = new GamesListResponse(games.Select(game => new GameMetaResponse(game.Game.Id, game.Game.Name, game.Game.IsPublic, game.Lobby.PlayerCount())).ToArray());
        return Ok(response);
    }

    [HttpGet("games/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameMetaResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDTO))]
    public IActionResult GetGames(long id)
    {
        var cookie = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        var activeSession = _userService.IsUserLoggedIn(cookie.UserId, cookie.SessionId);
        if (activeSession.User == null || activeSession.SessionId == null || activeSession.Error != null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(activeSession.Error), "user not logged in"));
        }

        ZwooRoom? game = _gamesService.GetGame(id);
        if (game == null)
        {
            return NotFound(ErrorCodes.GetResponse(ErrorCodes.Errors.GAME_NOT_FOUND, "game Not found"));
        }

        GameMetaResponse response = new GameMetaResponse(game.Game.Id, game.Game.Name, game.Game.IsPublic, game.Lobby.PlayerCount());
        return Ok(response);
    }
}
