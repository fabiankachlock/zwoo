using log4net;
using System.Net.WebSockets;
using System.Text;
using ZwooBackend.Games;
using ZwooBackend.ZRP;
using ZwooGameLogic.ZRP;

namespace ZwooBackend.Websockets;


public interface IWebSocketHandler : IDisposable
{
    public void Handle(long gameId, long userId, WebSocket ws, CancellationToken token, TaskCompletionSource source);
}


public class WebSocketHandler : IWebSocketHandler
{

    private ILog _logger;
    private IGameLogicService _gamesService;
    private bool disposedValue;

    public WebSocketHandler(IGameLogicService gamesService)
    {
        _gamesService = gamesService;
        _logger = LogManager.GetLogger("WebSocketHandler");
    }

    public async void Handle(long gameId, long userId, WebSocket webSocket, CancellationToken token, TaskCompletionSource source)
    {

        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        bool hasTooLongMessage = false;

        while (!receiveResult.CloseStatus.HasValue && !token.IsCancellationRequested)
        {
            // received message
            if (!receiveResult.EndOfMessage)
            {
                hasTooLongMessage = true;
            }
            else if (hasTooLongMessage)
            {
                await webSocket.SendAsync(ZRPEncoder.EncodeToBytes(ZRPCode.MessageToLongError, new ErrorDTO((int)ZRPCode.MessageToLongError, "message to long")), WebSocketMessageType.Text, true, CancellationToken.None);
                hasTooLongMessage = false;
            }
            else
            {
                _logger.Debug($"[{userId}] received message: {Encoding.UTF8.GetString(buffer, 0, receiveResult.Count)}");
                _distributeMessage(buffer, receiveResult.Count, gameId, userId);
            }

            try
            {
                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
            }
            catch (Exception ex)
            {
                if (webSocket.State != WebSocketState.Closed)
                {
                    _logger.Error($"[{userId}] error while receiving message:", ex);
                }
            }
        }

        _logger.Debug($"[{userId}] handler stopping");
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
            _logger.Error($"[{userId}] error while closing websocket:", ex);
        }

        source.SetResult();
        _logger.Debug($"[{userId}] handler closed");
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
            _logger.Warn($"[{userId}] received invalid ZRP message");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // no managed state
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}