using ZwooGameLogic.Lobby;

namespace ZwooGameLogic;

public class ZwooRoom
{
    public Game.Game Game;
    public LobbyManager Lobby;

    public ZwooRoom(Game.Game game, LobbyManager lobby)
    {
        Game = game;
        Lobby = lobby;
    }
}

