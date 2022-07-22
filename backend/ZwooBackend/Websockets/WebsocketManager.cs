using System.Text;
using System.Net.WebSockets;
using ZwooGameLogic.Game.Events;
using ZwooBackend.ZRP;
using ZwooBackend.Games;
using ZwooBackend.Websockets.Interfaces;

namespace ZwooBackend.Websockets;

public class WebSocketManager : SendableWebSocketManager, ManageableWebSocketManager
{
    private Dictionary<long, WebSocket> _websockets = new Dictionary<long, WebSocket>();
    private Dictionary<long, HashSet<long>> _games = new Dictionary<long, HashSet<long>>();
    private WebSocketMessageDistributer _distributer;

    public WebSocketManager()
    {
        _distributer = new WebSocketMessageDistributer(this);
    }


    public async void AddWebsocket(long gameId, long playerId, WebSocket ws, TaskCompletionSource closed)
    {
        try
        {
            InsertWs(gameId, playerId, ws);
        }
        catch
        {
            Globals.Logger.Warn($"cant store webSocket for {playerId}");
            closed.SetResult();
            return;
        }

        try
        {
            Globals.Logger.Info($"{playerId} connected!");
            await Handle(ws, playerId, gameId);
            Globals.Logger.Info($"{playerId} closing socket!");
        }
        catch { }

        RemoveWs(gameId, playerId, ws);
        GameRecord? game = GameManager.Global.GetGame(gameId);
        if (game != null)
        {
            game.Lobby.RemovePlayer(playerId);
        }
        closed.SetResult();
    }

    private void InsertWs(long gameId, long playerId, WebSocket ws)
    {
        Globals.Logger.Warn($"storing webSocket for {playerId}");
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
        Globals.Logger.Info($"removing webSocket from {playerId}");
        if (_websockets.ContainsKey(playerId))
        {
            _websockets.Remove(playerId);
        }

        if (_games.ContainsKey(gameId))
        {
            _games[gameId].Remove(playerId);
        }
    }

    public async Task SendPlayer(long playerId, ArraySegment<byte> content, WebSocketMessageType messageType, bool isEndOfMessage)
    {
        if (_websockets.ContainsKey(playerId))
        {
            await _websockets[playerId].SendAsync(content, messageType, isEndOfMessage, CancellationToken.None);
        }
    }

    public async Task BroadcastGame(long gameId, ArraySegment<byte> content, WebSocketMessageType messageType, bool isEndOfMessage)
    {
        if (_games.ContainsKey(gameId))
        {
            await Task.WhenAll(_games[gameId].Select(player => _websockets[player].SendAsync(content, messageType, isEndOfMessage, CancellationToken.None)));
        }
    }

    public NotificationManager GetNotificationManager(long gameId)
    {
        return new WebSocketNotificationManager(this, gameId);
    }

    public bool HasWebsocket(long playerId)
    {
        return _websockets.ContainsKey(playerId);
    }

    private async Task Handle(WebSocket webSocket, long playerId, long gameId)
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
                await SendPlayer(playerId, ZRPEncoder.EncodeToBytes(ZRPCode.MessagetoLongError, new ErrorDTO((int)ZRPCode.MessagetoLongError, "message to long")), WebSocketMessageType.Text, true);
                hasTooLongMessage = false;
            }
            else
            {
                Globals.Logger.Info($" {playerId} received message: {Encoding.UTF8.GetString(buffer, 0, receiveResult.Count)}");
                _distributer.Distribute(buffer, receiveResult.Count, playerId, gameId);
            }

            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}
