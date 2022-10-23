using System.Text;
using System.Net.WebSockets;
using ZwooGameLogic.Game.Events;
using ZwooBackend.ZRP;
using ZwooBackend.Games;
using ZwooBackend.Websockets.Interfaces;
using static ZwooBackend.Globals;

namespace ZwooBackend.Websockets;

public class WebSocketManager : SendableWebSocketManager, ManageableWebSocketManager
{
    private Dictionary<long, WebSocket> _websockets = new Dictionary<long, WebSocket>();
    private Dictionary<long, HashSet<long>> _games = new Dictionary<long, HashSet<long>>();
    private WebSocketMessageDistributer _distributer;
    private PlayerManager _playerManager;

    public WebSocketManager()
    {
        _distributer = new WebSocketMessageDistributer(this);
        _playerManager = new PlayerManager(this);
    }


    public async void AddWebsocket(long gameId, long playerId, WebSocket ws, TaskCompletionSource closed)
    {
        try
        {
            InsertWs(gameId, playerId, ws);
        }
        catch
        {
            WebSocketLogger.Warn($"cant store websocket for {playerId}");
            closed.SetResult();
            return;
        }


        var (player, game) = await _playerManager.ConnectPlayer(playerId, gameId);
        if (player == null || game == null)
        {
            WebSocketLogger.Warn($"no game found for {playerId}");
            closed.SetResult();
            return;
        }

        try
        {
            WebSocketLogger.Info($"{playerId} connected");
            await Handle(ws, player, game);
            WebSocketLogger.Info($"{playerId} closing socket");
        }
        catch (Exception e)
        {
            WebSocketLogger.Error($"in websocket {playerId}", e);
        }

        RemoveWs(gameId, playerId, ws);
        await _playerManager.DisconnectPlayer(playerId, gameId);
        WebSocketLogger.Info($"{playerId} disconnected");
        closed.SetResult();
    }

    private void InsertWs(long gameId, long playerId, WebSocket ws)
    {
        WebSocketLogger.Info($"storing websocket for {playerId}");
        if (!_websockets.ContainsKey(playerId))
        {
            _websockets[playerId] = ws;
        }
        else
        {
            throw new Exception();
        }

        if (_games.ContainsKey(gameId))
        {
            _games[gameId].Add(playerId);
        }
        else
        {
            _games[gameId] = new HashSet<long> { playerId };
        }
    }

    private void RemoveWs(long gameId, long playerId, WebSocket ws)
    {
        WebSocketLogger.Info($"removing websocket from {playerId}");
        if (_websockets.ContainsKey(playerId))
        {
            _websockets.Remove(playerId);
        }

        if (_games.ContainsKey(gameId))
        {
            _games[gameId].Remove(playerId);
            if (_games[gameId].Count == 0)
            {
                _games.Remove(gameId);
            }
        }
    }

    public async Task SendPlayer(long playerId, ArraySegment<byte> content, WebSocketMessageType messageType = WebSocketMessageType.Text, bool isEndOfMessage = true)
    {
        // TODO: not thread save (_websockets should be locked)
        if (_websockets.ContainsKey(playerId))
        {
            WebSocketLogger.Info($"[Player] [{playerId}] sending message");
            WsLogger.Debug($"[Player] [{playerId}] sending: {Encoding.UTF8.GetString(content)}");
            try
            {
                await _websockets[playerId].SendAsync(content, messageType, isEndOfMessage, CancellationToken.None);
            }
            catch (Exception e)
            {
                WebSocketLogger.Warn($"[Player] [{playerId}] error while sending a message {e}");
            }
        }
    }

    public async Task BroadcastGame(long gameId, ArraySegment<byte> content, WebSocketMessageType messageType = WebSocketMessageType.Text, bool isEndOfMessage = true)
    {
        // TODO: not thread save (_games should be locked)
        if (_games.ContainsKey(gameId))
        {
            try
            {
                WebSocketLogger.Info($"[Game] [{gameId}] broadcasting");
                WsLogger.Debug($"[Game] [{gameId}] sending: {Encoding.UTF8.GetString(content)}");
                await Task.WhenAll(_games[gameId].Select(async player =>
                {
                    if (_websockets.ContainsKey(player) && _websockets[player].State == WebSocketState.Open)
                    {
                        try
                        {
                            await _websockets[player].SendAsync(content, messageType, isEndOfMessage, CancellationToken.None);
                        }
                        catch (Exception e)
                        {
                            WebSocketLogger.Warn($"[Game] [{gameId}] error while sending a message {e}");
                        }
                    }
                }));
            }
            catch (Exception e)
            {
                WebSocketLogger.Warn($"[Game] [{gameId}] error while sending a message {e}");
            }
        }
    }

    public NotificationManager GetNotificationManager(long gameId)
    {
        // TODO: optimize this
        return new WebSocketNotificationManager(this, gameId, id =>
            {
                if (GameManager.Global.HasGame(gameId))
                {
                    var lobby = GameManager.Global.GetGame(gameId)!.Lobby;
                    return lobby.GetPlayer(id)!.Username;
                }
                throw new Exception("unknown game");
            });
    }

    public bool HasWebsocket(long playerId)
    {
        return _websockets.ContainsKey(playerId);
    }

    private async Task Handle(WebSocket webSocket, LobbyManager.PlayerEntry player, GameRecord game)
    {

        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        bool hasTooLongMessage = false;

        while (!receiveResult.CloseStatus.HasValue)
        {
            // received message
            if (!receiveResult.EndOfMessage)
            {
                hasTooLongMessage = true;
            }
            else if (hasTooLongMessage)
            {
                await SendPlayer(player.Id, ZRPEncoder.EncodeToBytes(ZRPCode.MessagetoLongError, new ErrorDTO((int)ZRPCode.MessagetoLongError, "message to long")), WebSocketMessageType.Text, true);
                hasTooLongMessage = false;
            }
            else
            {
                WsLogger.Debug($"{player.Id} received message: {Encoding.UTF8.GetString(buffer, 0, receiveResult.Count)}");
                _distributer.Distribute(buffer, receiveResult.Count, player, game.Game.Id, game);
            }

            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        if (webSocket.CloseStatus == null)
        {
            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }

    public async Task Disconnect(long playerId)
    {
        try
        {
            WebSocketLogger.Info($"forcing disconnect for {playerId}");
            WebSocket webSocket = _websockets[playerId];
            await webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "you got kicked",
                CancellationToken.None);
            return;
        }
        catch { }
        return;
    }

    public async Task QuitGame(long gameId)
    {
        if (_games.ContainsKey(gameId))
        {
            WebSocketLogger.Info($"closing game {gameId}");
            HashSet<long> players = _games[gameId];
            foreach (long player in players)
            {
                await Disconnect(player);
            }
        }
    }

    public async Task FinishGame(long gameId)
    {
        await _playerManager.FinishGame(gameId);
    }
}
