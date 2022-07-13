using System.Net.WebSockets;

namespace ZwooBackend.Websockets.Interfaces;

public interface SendableWebSocketManager
{
    public Task SendPlayer(long playerId, ArraySegment<byte> content, WebSocketMessageType messageType, bool isEndOfMessage);

    public Task BroadcastGame(long gameId, ArraySegment<byte> content, WebSocketMessageType messageType, bool isEndOfMessage);
}