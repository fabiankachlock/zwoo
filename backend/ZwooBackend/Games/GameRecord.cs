using ZwooGameLogic.Game;

namespace ZwooBackend.Games;

public class GameRecord
{
    public Game Game;
    public LobbyManager Lobby;

    public GameRecord(Game game, LobbyManager lobby)
    {
        Game = game;
        Lobby = lobby;
    }
}
