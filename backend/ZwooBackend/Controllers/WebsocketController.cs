using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ZwooBackend.Websockets;
using System.Net.WebSockets;
using System.Net.Http;
using System.Text;
using ZwooBackend.Games;


namespace ZwooBackend.Controllers;

[ApiController]
public class WebSocketController : Controller
{

    [HttpGet]
    [Route("/game/join/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task ConnectWebsocket(int id)
    {
        Globals.Logger.Info($"GET /game/join/{id}");
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            byte[] response;

            if (CookieHelper.CheckUserCookie(HttpContext.User.FindFirst("auth")?.Value, out var user, out _))
            {
                GameRecord? game = GameManager.Global.GetGame(id);

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
                TaskCompletionSource finished = new TaskCompletionSource();

                GameManager.Global.WebSocketManager.AddWebsocket(id, (long)user.Id, webSocket, finished);
                await finished.Task;
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
