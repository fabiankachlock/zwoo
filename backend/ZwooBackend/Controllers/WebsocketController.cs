using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ZwooBackend.Websockets;
using System.Net.WebSockets;
using System.Net.Http;


namespace ZwooBackend.Controllers;

[ApiController]
public class WebSocketController : Controller
{
    private Websockets.WebSocketManager _websocketManager = new Websockets.WebSocketManager();

    [EnableCors("zwoo-cors")]
    [HttpGet]
    [Route("/version")]
    public string Test()
    {
        return "hello world";
    }

    [HttpGet]
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
