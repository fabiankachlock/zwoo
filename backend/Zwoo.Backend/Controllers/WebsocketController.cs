using log4net;
using Microsoft.AspNetCore.Mvc;
using Zwoo.Backend.Websockets;
using Zwoo.Backend.Games;
using System.Net.WebSockets;
using Zwoo.GameEngine;
using Zwoo.GameEngine.Lobby;
using Zwoo.Database;
using Zwoo.Backend.Controllers.DTO;



namespace Zwoo.Backend.Controllers;

[ApiController]
public class WebSocketController : Controller
{
    private IWebSocketManager _wsManager;
    private IWebSocketHandler _wsHandler;
    private IGameLogicService _gamesService;
    private IUserService _userService;


    private ILog _logger;

    public WebSocketController(IWebSocketManager wsManager, IWebSocketHandler wsHandler, IGameLogicService gamesService, IUserService userService)
    {
        _wsHandler = wsHandler;
        _wsManager = wsManager;
        _gamesService = gamesService;
        _userService = userService;
        _logger = LogManager.GetLogger("PlayerLifetime");
    }


    [HttpGet]
    [Route("/game/join/{gameId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDTO))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> ConnectWebsocket(int gameId)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            return BadRequest(new ErrorDTO() { Message = "only ws requests accepted" });
        }

        var cookie = CookieHelper.ParseCookie(HttpContext.User.FindFirst("auth")?.Value ?? "");
        var activeSession = _userService.IsUserLoggedIn(cookie.UserId, cookie.SessionId);
        if (activeSession.User == null || activeSession.SessionId == null || activeSession.Error != null)
        {
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.FromDatabaseError(activeSession.Error), "activeSession.User not logged in"));
        }

        ZwooRoom? game = _gamesService.GetGame(gameId);
        if (game == null)
        {
            return NotFound(ErrorCodes.GetResponse(ErrorCodes.Errors.GAME_NOT_FOUND, "game not found"));
        }

        LobbyResult result = game.Lobby.IsPlayerAllowedToConnect((long)activeSession.User.Id);
        if (result != LobbyResult.Success)
        {
            if (result == LobbyResult.ErrorLobbyFull)
            {
                return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.GAME_FULL, "lobby is full"));
            }
            else if (result == LobbyResult.ErrorWrongPassword)
            {
                return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.INVALID_PASSWORD, "wrong password"));
            }
            return Unauthorized(ErrorCodes.GetResponse(ErrorCodes.Errors.JOIN_FAILED, "not allowed to join"));
        }

        _logger.Info($"[{activeSession.User.Id}] accepting websocket");
        WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        CancellationTokenSource shouldStop = new CancellationTokenSource();
        TaskCompletionSource finished = new TaskCompletionSource();

        _logger.Info($"[{activeSession.User.Id}] adding connection");
        bool success = _wsManager.AddConnection(gameId, (long)activeSession.User.Id, webSocket, shouldStop);
        if (!success)
        {
            _logger.Error($"[{activeSession.User.Id}] cant add connection");
            return StatusCode(StatusCodes.Status500InternalServerError, "internal error while handling the connection");
        }

        _logger.Info($"[{activeSession.User.Id}] notify playermanager");
        await game.PlayerManager.ConnectPlayer((long)activeSession.User.Id);

        _logger.Info($"[{activeSession.User.Id}] handle");
        try
        {
            _wsHandler.Handle(gameId, (long)activeSession.User.Id, webSocket, shouldStop.Token, finished);
        }
        catch (TaskCanceledException)
        {
            _logger.Info($"[{activeSession.User.Id}] task cancelled");
        }
        catch (Exception ex)
        {
            _logger.Warn($"[{activeSession.User.Id}] an error happened while handling a socket", ex);
        }
        await finished.Task;

        _logger.Info($"[{activeSession.User.Id}] remove connection");
        success = _wsManager.RemoveConnection(gameId, (long)activeSession.User.Id);
        if (!success)
        {
            _logger.Error($"[{activeSession.User.Id}] cant remove connection");
        }

        _logger.Info($"[{activeSession.User.Id}] disconnect");
        await game.PlayerManager.DisconnectPlayer((long)activeSession.User.Id);
        return new EmptyResult();
    }
}
