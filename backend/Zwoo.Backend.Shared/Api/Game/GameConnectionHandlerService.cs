using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zwoo.Backend.Shared.Api.Game.Adapter;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.Backend.Shared.Api.Game;

public interface IGameConnectionHandlerService
{
    /// <summary>
    /// handle a websocket connect
    /// </summary>
    /// <param name="gameId">the associated games id</param>
    /// <param name="userId">the associated users id</param>
    /// <param name="ws">the websocket object</param>
    /// <param name="token">the connection cancellation token</param>
    /// <param name="source">a task completion source waiting until the connection is closed</param>
    public void Handle(long gameId, long userId, WebSocket ws, CancellationToken token, TaskCompletionSource source);
}

public class GameConnectionHandlerService : IGameConnectionHandlerService
{
    private ILogger<GameConnectionHandlerService> _logger;
    private IGameEngineService _gamesService;
    private IHostApplicationLifetime _applicationLifetime;

    public GameConnectionHandlerService(IGameEngineService gamesService, IHostApplicationLifetime applicationLifetime, ILogger<GameConnectionHandlerService> logger)
    {
        _gamesService = gamesService;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }

    public async void Handle(long gameId, long userId, WebSocket webSocket, CancellationToken token, TaskCompletionSource source)
    {

        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        bool hasTooLongMessage = false;
        var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _applicationLifetime.ApplicationStopping);

        _logger.LogDebug($"[{userId}] starting handler");
        while (!receiveResult.CloseStatus.HasValue && !combinedTokenSource.IsCancellationRequested)
        {
            // received message
            if (!receiveResult.EndOfMessage)
            {
                hasTooLongMessage = true;
            }
            else if (hasTooLongMessage)
            {
                _logger.LogWarning($"[{userId}] received too long message");
                await webSocket.SendAsync(ZRPEncoder.EncodeToBytes(ZRPCode.MessageToLongError, new Error((int)ZRPCode.MessageToLongError, "message to long")), WebSocketMessageType.Text, true, CancellationToken.None);
                hasTooLongMessage = false;
            }
            else
            {
                _logger.LogWarning($"[{userId}] received message");
                _logger.LogTrace($"[{userId}] raw message: {Encoding.UTF8.GetString(buffer, 0, receiveResult.Count)}");
                _distributeMessage(buffer, receiveResult.Count, gameId, userId);
            }

            try
            {
                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), combinedTokenSource.Token);
            }
            catch (Exception ex)
            {
                if (webSocket.State != WebSocketState.Closed)
                {
                    _logger.LogError($"[{userId}] error while receiving message", ex);
                }
            }
        }

        _logger.LogDebug($"[{userId}] stopping handler");
        try
        {
            if (webSocket.CloseStatus == null && webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(
                    receiveResult.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                    receiveResult.CloseStatusDescription,
                    CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{userId}] error while closing websocket", ex);
        }

        source.SetResult();
        _logger.LogDebug($"[{userId}] handler closed");
    }

    private void _distributeMessage(byte[] message, int length, long gameId, long userId)
    {
        string stringMessage = Encoding.UTF8.GetString(message, 0, length);
        ZRPCode? code = ZRPDecoder.GetCode(stringMessage);
        if (code != null)
        {
            ZRPMessage zrpMessage = new ZRPMessage(userId, code.Value, stringMessage);
            _gamesService.DistributeEvent(gameId, zrpMessage);
        }
        else
        {
            _logger.LogWarning($"[{userId}] received invalid ZRP message");
        }
    }
}