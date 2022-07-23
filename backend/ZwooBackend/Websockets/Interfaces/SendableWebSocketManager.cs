using System.Net.WebSockets;

namespace ZwooBackend.Websockets.Interfaces;

public interface SendableWebSocketManager
{
    public Task SendPlayer(long playerId, ArraySegment<byte> content, WebSocketMessageType messageType = WebSocketMessageType.Text, bool isEndOfMessage = true);

    public Task BroadcastGame(long gameId, ArraySegment<byte> content, WebSocketMessageType messageType = WebSocketMessageType.Text, bool isEndOfMessage = true);

    public Task Disconnect(long playerdId);
}