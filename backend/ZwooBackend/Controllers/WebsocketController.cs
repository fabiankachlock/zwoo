using Microsoft.AspNetCore.Mvc;
using ZwooBackend.Websockets;
using System.Net.WebSockets;


namespace ZwooBackend.Controllers;

[ApiController]
public class WebSocketController : Controller
{
    private Websockets.WebSocketManager _websocketManager = new Websockets.WebSocketManager();


    [Route("/game/join/{id}")]
    public async Task Index(int gameId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            TaskCompletionSource finished = new TaskCompletionSource();
            // TODO: get player id from auth
            _websocketManager.AddWebsocket(gameId, 0, webSocket, finished);
            await finished.Task;
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }


}
