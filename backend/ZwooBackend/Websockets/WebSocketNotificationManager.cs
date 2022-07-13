using ZwooBackend.Websockets.Interfaces;
using ZwooGameLogic.Game.Events;

namespace ZwooBackend.Websockets;

public class WebSocketNotificationManager : NotificationManager
{

    private SendableWebSocketManager _webSockets;

    private long _gameId;

    public WebSocketNotificationManager(SendableWebSocketManager webSocketManager, long gameId)
    {
        _webSockets = webSocketManager;
        _gameId = gameId;
    }

    public void EndTurn(long player)
    {
        throw new NotImplementedException();
    }

    public void GetPlayerDecission(PlayerDecissionDTO data)
    {
        throw new NotImplementedException();
    }

    public void PlayerWon(PlayerWonDTO data)
    {
        throw new NotImplementedException();
    }

    public void RemoveCard(RemoveCardDTO data)
    {
        throw new NotImplementedException();
    }

    public void SendCard(SendCardDTO data)
    {
        throw new NotImplementedException();
    }

    public void StartGame()
    {
        throw new NotImplementedException();
    }

    public void StartTurn(long player)
    {
        throw new NotImplementedException();
    }

    public void StateUpdate(StateUpdateDTO data)
    {
        throw new NotImplementedException();
    }

    public void StopGame()
    {
        throw new NotImplementedException();
    }
}
