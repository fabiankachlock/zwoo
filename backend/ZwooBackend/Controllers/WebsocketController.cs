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

    public WebSocketController(IWebSocketManager wsManager, IWebSocketHandler wsHandler, IGameLogicService gamesService)
    {
        _wsHandler = wsHandler;
        _wsManager = wsManager;
        _gamesService = gamesService;
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

                LobbyResult result = game.Lobby.PlayerWantsToConnect((long)user.Id);
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

                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                bool success = _wsManager.AddConnection(gameId, (long)user.Id, webSocket);
                if (!success) return; // TODO: logging

                await game.PlayerManager.ConnectPlayer((long)user.Id);
                await _wsHandler.Handle(gameId, (long)user.Id, webSocket);

                await game.PlayerManager.DisconnectPlayer((long)user.Id);
                success = _wsManager.RemoveConnection(gameId, (long)user.Id);
                if (!success) return; // TODO: logging


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
