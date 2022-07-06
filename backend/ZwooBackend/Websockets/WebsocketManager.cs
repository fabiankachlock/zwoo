using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;


namespace ZwooBackend.Websockets;

public class WebSocketManager
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
