using ZwooGameLogic;
using ZwooBackend;

namespace ZwooBackend.Games;

internal class GameGlobals
{
    static internal readonly Websockets.WebSocketManager WebSocketManager = new Websockets.WebSocketManager();

    static internal readonly GameManager GameManager = new GameManager(id => WebSocketManager.GetNotificationManager(id));
}
