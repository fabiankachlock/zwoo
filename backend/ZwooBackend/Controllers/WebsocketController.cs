using log4net;
using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Websockets;
using ZwooBackend.Games;
using System.Net.WebSockets;
using System.Text;
using ZwooGameLogic;
using ZwooGameLogic.Lobby;


namespace ZwooBackend.Controllers;

[ApiController]
public class WebSocketController : Controller
{
    private IWebSocketManager _wsManager;
    private IWebSocketHandler _wsHandler;
    private IGameLogicService _gamesService;

    private ILog _logger;

    public WebSocketController(IWebSocketManager wsManager, IWebSocketHandler wsHandler, IGameLogicService gamesService)
    {
        _wsHandler = wsHandler;
        _wsManager = wsManager;
        _gamesService = gamesService;
        _logger = LogManager.GetLogger("PlayerLifetime");
    }


    [HttpGet]
    [Route("/game/join/{gameId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task ConnectWebsocket(int gameId)
    {
        Globals.Logger.Info($"GET /game/join/{gameId}");
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            byte[] response;

            if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user, out _))
            {
                ZwooRoom? game = _gamesService.GetGame(gameId);

                if (game == null)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    response = Encoding.UTF8.GetBytes(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.GAME_NOT_FOUND, "game not found"));
                    await HttpContext.Response.Body.WriteAsync(response, 0, response.Length);
                    return;
                }

                LobbyResult result = game.Lobby.IsPlayerAllowedToConnect((long)user.Id);
                if (result != LobbyResult.Success)
                {
                    if (result == LobbyResult.ErrorLobbyFull)
                    {
                        response = Encoding.UTF8.GetBytes(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.GAME_FULL, "lobby is full"));
                    }
                    else if (result == LobbyResult.ErrorWrongPassword)
                    {
                        response = Encoding.UTF8.GetBytes(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.INVALID_PASSWORD, "wrong password"));
                    }
                    else
                    {
                        response = Encoding.UTF8.GetBytes(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.JOIN_FAILED, "not allowed to join"));
                    }
                    HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await HttpContext.Response.Body.WriteAsync(response, 0, response.Length);
                    return;
                }

                _logger.Info($"[{user.Id}] accepting websocket");
                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                CancellationTokenSource shouldStop = new CancellationTokenSource();
                TaskCompletionSource finished = new TaskCompletionSource();

                _logger.Info($"[{user.Id}] adding connection");
                bool success = _wsManager.AddConnection(gameId, (long)user.Id, webSocket, shouldStop);
                if (!success)
                {
                    _logger.Error($"[{user.Id}] cant add connection");
                    return;
                }

                _logger.Info($"[{user.Id}] notify playermanager");
                await game.PlayerManager.ConnectPlayer((long)user.Id);

                _logger.Info($"[{user.Id}] handle");
                _wsHandler.Handle(gameId, (long)user.Id, webSocket, shouldStop.Token, finished);
                await finished.Task;

                _logger.Info($"[{user.Id}] remove connection");
                success = _wsManager.RemoveConnection(gameId, (long)user.Id);
                if (!success)
                {
                    _logger.Error($"[{user.Id}] cant remove connection");
                }

                _logger.Info($"[{user.Id}] disconnect");
                await game.PlayerManager.DisconnectPlayer((long)user.Id);

                return;
            }
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            response = Encoding.UTF8.GetBytes(ErrorCodes.GetErrorResponseMessage(ErrorCodes.Errors.SESSION_ID_NOT_MATCHING, "Unauthorized"));
            await HttpContext.Response.Body.WriteAsync(response, 0, response.Length);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
