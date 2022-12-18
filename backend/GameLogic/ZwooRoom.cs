using ZwooGameLogic.Lobby;

namespace ZwooGameLogic;

public class ZwooRoom
{
    public readonly Game.Game Game;
    public readonly LobbyManager Lobby;
    public readonly GameMessageDistributer EventDistributer;
    public readonly ZRPPlayerManager PlayerManager;


    public delegate void ClosedHandler();
    public event ClosedHandler OnClosed = delegate { };

    public long Id
    {
        get => Game.Id;
    }

    public ZwooRoom(Game.Game game, LobbyManager lobby, INotificationAdapter notificationAdapter)
    {
        Game = game;
        Lobby = lobby;
        EventDistributer = new GameMessageDistributer(notificationAdapter);
        PlayerManager = new ZRPPlayerManager(notificationAdapter, this);

        Game.OnFinished += async () => await PlayerManager.FinishGame();
    }

    public void Close()
    {
        OnClosed.Invoke();
    }
}

