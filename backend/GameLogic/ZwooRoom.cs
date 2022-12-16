using ZwooGameLogic.Lobby;

namespace ZwooGameLogic;

public class ZwooRoom
{
    public Game.Game Game { get; }
    public LobbyManager Lobby { get; }

    public long Id
    {
        get => Game.Id;
    }

    public ZwooRoom(Game.Game game, LobbyManager lobby)
    {
        Game = game;
        Lobby = lobby;
    }
}

