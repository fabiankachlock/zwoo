using System.Net.WebSockets;


namespace ZwooBackend.Websockets.Interfaces;


public interface ManageableWebSocketManager
{

    public void AddWebsocket(long gameId, long playerId, WebSocket ws, TaskCompletionSource closed);
}
