using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using ZwooGameLogic.Game.Events;
using ZwooBackend.Websockets.Interfaces;

namespace ZwooBackend.Websockets;

public class WebSocketManager : SendableWebSocketManager, ManageableWebSocketManager
{
    private Dictionary<long, WebSocket> _websockets = new Dictionary<long, WebSocket>();
    private Dictionary<long, HashSet<long>> _games = new Dictionary<long, HashSet<long>>();

    public async void AddWebsocket(long gameId, long playerId, WebSocket ws, TaskCompletionSource closed)
    {
        try
        {
            InsertWs(gameId, playerId, ws);
        }
        catch (Exception e)
        {
            closed.SetResult();
            return;
        }

        try
        {
            await Echo(ws);
        }
        catch (Exception e) { }

        RemoveWs(gameId, playerId, ws);
        closed.SetResult();
    }

    private void InsertWs(long gameId, long playerId, WebSocket ws)
    {
        if (_websockets.ContainsKey(playerId))
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

    private static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

        /*while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }*/

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}
