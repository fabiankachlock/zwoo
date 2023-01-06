using log4net;
using System.Net.WebSockets;
using System.Text;
using ZwooGameLogic.ZRP;
using ZwooBackend.ZRP;
using ZwooGameLogic.Notifications;


namespace ZwooBackend.Websockets;

public interface IWebSocketManager : IDisposable, INotificationAdapter
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

public class WebSocketManager : IWebSocketManager
{

    private struct Entry
    {
        public WebSocket WebSocket;
        public CancellationTokenSource CancellationToken;
    }

    private ILog _logger;
    private Dictionary<long, Entry> _websockets = new Dictionary<long, Entry>();
    private Dictionary<long, HashSet<long>> _games = new Dictionary<long, HashSet<long>>();
    private bool disposedValue;

    public WebSocketManager()
    {
        _logger = LogManager.GetLogger("WebSocketManager");
    }

    public bool AddConnection(long gameId, long playerId, WebSocket ws, CancellationTokenSource tokenSource)
    {
        _logger.Info($"storing websocket for {playerId}");
        lock (_websockets)
        {
            // store websocket
            if (!_websockets.ContainsKey(playerId))
            {
                _websockets[playerId] = new Entry()
                {
                    WebSocket = ws,
                    CancellationToken = tokenSource,
                };
            }
            else
            {
                _logger.Warn($"cant store websocket for {playerId}");
                return false;
            }
        }

        lock (_games)
        {
            // store game reference
            if (_games.ContainsKey(gameId))
            {
                _games[gameId].Add(playerId);
            }
            else
            {
                _games[gameId] = new HashSet<long> { playerId };
            }
        }
        return true;
    }

    public bool RemoveConnection(long gameId, long playerId)
    {
        _logger.Info($"removing websocket from {playerId}");
        bool wasRemoved = false;
        lock (_websockets)
        {
            if (_websockets.ContainsKey(playerId))
            {
                wasRemoved = _websockets.Remove(playerId);
            }
        }

        lock (_games)
        {
            if (_games.ContainsKey(gameId))
            {
                _games[gameId].Remove(playerId);
                if (_games[gameId].Count == 0)
                {
                    _games.Remove(gameId);
                }
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

    public async Task<bool> SendPlayer(long playerId, ArraySegment<byte> content, WebSocketMessageType messageType = WebSocketMessageType.Text, bool isEndOfMessage = true)
    {
        Task sendingTask = Task.CompletedTask;
        lock (_websockets)
        {
            if (!_websockets.ContainsKey(playerId))
            {
                _logger.Warn($"[Player] [{playerId}] cant send message to unknown player");
                return false;
            }

            if (_websockets[playerId].WebSocket.State != WebSocketState.Open)
            {
                _logger.Warn($"[Player] [{playerId}] send message since it is not open");
                return false;
            }

            _logger.Info($"[Player] [{playerId}] sending message");
            _logger.Debug($"[Player] [{playerId}] sending: {Encoding.UTF8.GetString(content)}");
            sendingTask = _websockets[playerId].WebSocket.SendAsync(content, messageType, isEndOfMessage, CancellationToken.None);
        }

        try
        {
            await sendingTask;
        }
        catch (Exception ex)
        {
            _logger.Warn($"[Player] [{playerId}] error while sending a message", ex);
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
                _logger.Warn($"[Game] [{gameId}] cant broadcast to unknown game");
                return false;
            }

            _logger.Info($"[Game] [{gameId}] broadcasting");
            sendingTask = Task.WhenAll(_games[gameId].Select(player => SendPlayer(player, content, messageType, isEndOfMessage)));
        }

        try
        {
            await sendingTask;
        }
        catch (Exception ex)
        {
            _logger.Warn($"[Game] [{gameId}] error while sending a message", ex);
        }
        return true;
    }

    public async Task<bool> DisconnectPlayer(long playerId)
    {
        Task messageTask;
        lock (_websockets)
        {
            _logger.Info($"forcing disconnect for {playerId}");
            if (!_websockets.ContainsKey(playerId)) return false;
            // stop handler
            _websockets[playerId].CancellationToken.Cancel();
            WebSocket webSocket = _websockets[playerId].WebSocket;
            if (webSocket.State != WebSocketState.Open)
            {
                _logger.Warn($"[Player] [{playerId}] cant close websocket since it is not open");
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
            _logger.Warn($"error happened while closing the connection of {playerId}", ex);
            return false;
        }
        return true;
    }

    public async Task<bool> DisconnectGame(long gameId)
    {
        Task messageTask;
        lock (_games)
        {
            _logger.Info($"forcing disconnect for game {gameId}");
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
            _logger.Warn($"error happened while closing the connection of game {gameId}", ex);
            return false;
        }
        return true;
    }

    protected virtual void Dispose(bool disposing)
    {
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
