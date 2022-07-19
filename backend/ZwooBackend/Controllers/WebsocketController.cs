using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ZwooBackend.Websockets;
using System.Net.WebSockets;
using System.Net.Http;
using ZwooBackend.Games;


namespace ZwooBackend.Controllers;

[ApiController]
public class WebSocketController : Controller
{

    [HttpGet]
    [Route("/game/join/{id}")]
    public async Task Index(int gameId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            TaskCompletionSource finished = new TaskCompletionSource();
            // TODO: get player id from auth
            // GameGlobals.WebSocketManager.AddWebsocket(gameId, 0, webSocket, finished);
            await finished.Task;
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }


}
