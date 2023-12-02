using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Zwoo.Backend.Shared.Api.Game.Adapter;
using Zwoo.GameEngine.Notifications;
using Zwoo.GameEngine.ZRP;

namespace Zwoo.Backend.Shared.Api.Game;

public interface IGameConnectionsService : IDisposable, INotificationAdapter
{
    /// <summary>
    /// register a new active connection
    /// </summary>
    /// <param name="gameId">the id of the game</param> 
    /// <param name="playerId">the id of the player</param>
    /// <param name="ws">a websocket object</param>
    /// <returns></returns>
    public bool AddConnection(long gameId, long playerId, WebSocket ws, CancellationTokenSource tokenSource);

    /// <summary>
    /// remove an active websocket connection
    /// </summary>
    /// <param name="gameId">the id of the game</param>
    /// <param name="playerId">the id of the player</param>
    /// <returns>whether a connection was removed or not</returns>
    public bool RemoveConnection(long gameId, long playerId);

    /// <summary>
    /// returns if a player has already an active websocket connection
    /// </summary>
    /// <param name="playerId">the players id</param>
    /// <returns></returns>
    public bool HasConnection(long playerId);

    /// <summary>
    /// returns the amount of active connections for a game
    /// </summary>
    /// <param name="gameId">the id of the game</param>
    /// <returns>the amount if active connections</returns>
    public int ConnectionsCount(long gameId);

    /// <summary>
    /// handle a websocket connect
    /// </summary>
    /// <param name="gameId">the associated games id</param>
    /// <param name="userId">the associated users id</param>
    /// <param name="ws">the websocket object</param>
    /// <param name="token">the connection cancellation token</param>
    /// <param name="source">a task completion source waiting until the connection is closed</param>
    public void Handle(long gameId, long userId, WebSocket ws, CancellationToken token, TaskCompletionSource source);

    /// <summary>
    /// send a message to an individual player
    /// </summary>
    /// <param name="playerId">the players id</param>
    /// <param name="content">the content of the message</param>
    /// <param name="messageType">the message type</param>
    /// <param name="isEndOfMessage">bool indicating whether this is the end of a message</param>
    /// <returns>whether a message could be sent</returns>
    public Task<bool> SendPlayer(
        long playerId,
        ArraySegment<byte> content,
        WebSocketMessageType messageType = WebSocketMessageType.Text,
        bool isEndOfMessage = true);

    /// <summary>
    /// send a message to an entire game
    /// </summary>
    /// <param name="gameId">the games id</param>
    /// <param name="content">the content of the message</param>
    /// <param name="messageType">the message type</param>
    /// <param name="isEndOfMessage">bool indicating whether this is the end of a message</param>
    /// <returns>whether a message could be sent</returns>
    public Task<bool> BroadcastGame(
        long gameId,
        ArraySegment<byte> content,
        WebSocketMessageType messageType = WebSocketMessageType.Text,
        bool isEndOfMessage = true);
}

public class GameConnectionsService : IGameConnectionsService
{

    private struct PlayerEntry
    {
        public WebSocket WebSocket;
        public CancellationTokenSource CancellationToken;
    }

    private ILogger<GameConnectionsService> _logger;
    private IGameEngineService _gamesService;

    private Dictionary<long, PlayerEntry> _websockets = new Dictionary<long, PlayerEntry>();
    private Dictionary<long, HashSet<long>> _games = new Dictionary<long, HashSet<long>>();
    private bool disposedValue;

    public GameConnectionsService(IGameEngineService gamesService, ILogger<GameConnectionsService> logger)
    {
        _gamesService = gamesService;
        _logger = logger;
    }

    public bool AddConnection(long gameId, long userId, WebSocket ws, CancellationTokenSource tokenSource)
    {
        _logger.LogDebug($"[{userId}] storing websocket");
        lock (_websockets)
        {
            // store websocket
            if (!_websockets.ContainsKey(userId))
            {
                _websockets[userId] = new PlayerEntry()
                {
                    WebSocket = ws,
                    CancellationToken = tokenSource,
                };
            }
            else
            {
                _logger.LogWarning($"[{userId}] already stored websocket for this id");
                return false;
            }
        }

        lock (_games)
        {
            // store game reference
            if (_games.ContainsKey(gameId))
            {
                _logger.LogDebug($"[{userId}] adding to existing game set {gameId}");
                _games[gameId].Add(userId);
            }
            else
            {
                _logger.LogDebug($"[{userId}] creating a new game set {gameId}");
                _games[gameId] = new HashSet<long> { userId };
            }
        }
        return true;
    }

    public bool RemoveConnection(long gameId, long userId)
    {
        _logger.LogDebug($"[{userId}] removing websocket");
        bool wasRemoved = false;
        lock (_websockets)
        {
            if (_websockets.ContainsKey(userId))
            {
                wasRemoved = _websockets.Remove(userId);
            }
            else
            {
                _logger.LogWarning($"[{userId}] unknown websocket");
            }
        }

        lock (_games)
        {
            if (_games.ContainsKey(gameId))
            {
                _logger.LogDebug($"[{userId}] removing from game set {gameId}");
                _games[gameId].Remove(userId);
                if (_games[gameId].Count == 0)
                {
                    _logger.LogDebug($"[{userId}] last in game set {gameId}");
                    _games.Remove(gameId);
                }
            }
            else
            {
                _logger.LogWarning($"[{userId}] unknown game set {gameId}");
            }
        }
        return wasRemoved;
    }

    public bool HasConnection(long playerId)
    {
        return _websockets.ContainsKey(playerId);
    }

    public int ConnectionsCount(long gameId)
    {
        lock (_games)
        {
            if (!_games.ContainsKey(gameId)) return 0;
            return _games[gameId].Count();
        }
    }

    public async Task<bool> SendPlayer(long userId, ArraySegment<byte> content, WebSocketMessageType messageType = WebSocketMessageType.Text, bool isEndOfMessage = true)
    {
        Task sendingTask = Task.CompletedTask;
        lock (_websockets)
        {
            if (!_websockets.ContainsKey(userId))
            {
                _logger.LogWarning($"[{userId}] cant send message to unknown player");
                return false;
            }

            if (_websockets[userId].WebSocket.State != WebSocketState.Open)
            {
                _logger.LogWarning($"[{userId}] cant send message to closed websocket");
                return false;
            }

            _logger.LogDebug($"[{userId}] sending message");
            _logger.LogTrace($"[{userId}] raw message: {Encoding.UTF8.GetString(content)}");
            sendingTask = _websockets[userId].WebSocket.SendAsync(content, messageType, isEndOfMessage, CancellationToken.None);
        }

        try
        {
            await sendingTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{userId}] error while sending a message");
        }
        return true;
    }

    public async Task<bool> BroadcastGame(long gameId, ArraySegment<byte> content, WebSocketMessageType messageType = WebSocketMessageType.Text, bool isEndOfMessage = true)
    {
        Task sendingTask = Task.CompletedTask;
        lock (_games)
        {
            if (!_games.ContainsKey(gameId))
            {
                _logger.LogWarning($"[Game-{gameId}] cant broadcast to unknown game");
                return false;
            }

            _logger.LogDebug($"[Game-{gameId}] broadcasting to {_games[gameId].Count} websockets");
            sendingTask = Task.WhenAll(_games[gameId].Select(player => SendPlayer(player, content, messageType, isEndOfMessage)));
        }

        try
        {
            await sendingTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[Game-{gameId}] error while broadcasting a message");
        }
        return true;
    }

    public async Task<bool> DisconnectPlayer(long userId)
    {
        Task messageTask;
        lock (_websockets)
        {
            _logger.LogDebug($"[{userId}] forcing disconnect");
            if (!_websockets.ContainsKey(userId)) return false;
            // stop handler
            _websockets[userId].CancellationToken.Cancel();
            WebSocket webSocket = _websockets[userId].WebSocket;
            if (webSocket.State != WebSocketState.Open)
            {
                _logger.LogWarning($"[{userId}] websocket is already closed");
                return false;
            }

            messageTask = webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "forced closing", CancellationToken.None);
        }

        try
        {
            await messageTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{userId}] error happened while closing the connection");
            return false;
        }
        return true;
    }

    public async Task<bool> DisconnectGame(long gameId)
    {
        Task messageTask;
        lock (_games)
        {
            _logger.LogDebug($"[Game-{gameId}] forcing disconnect");
            if (!_games.ContainsKey(gameId)) return false;
            messageTask = Task.WhenAll(_games[gameId].Select(async player =>
            {
                await DisconnectPlayer(player);
            }));
        }

        try
        {
            await messageTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[Game-{gameId}] error happened while closing the game");
            return false;
        }
        return true;
    }

    public async void Handle(long gameId, long userId, WebSocket webSocket, CancellationToken token, TaskCompletionSource source)
    {

        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        bool hasTooLongMessage = false;

        _logger.LogDebug($"[{userId}] starting handler");
        while (!receiveResult.CloseStatus.HasValue && !token.IsCancellationRequested)
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
                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
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

    protected virtual void Dispose(bool disposing)
    {
        _logger.LogInformation("disposing websockets...");
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var websocketPair in _websockets)
                {
                    websocketPair.Value.WebSocket.Dispose();
                }
            }
            _websockets.Clear();
            _games.Clear();
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public Task<bool> SendPlayer<T>(long playerId, ZRPCode code, T payload)
    {
        return SendPlayer(playerId, ZRPEncoder.EncodeToBytes(code, payload), WebSocketMessageType.Text, true);
    }

    public Task<bool> BroadcastGame<T>(long gameId, ZRPCode code, T payload)
    {
        return BroadcastGame(gameId, ZRPEncoder.EncodeToBytes(code, payload), WebSocketMessageType.Text, true);
    }
}