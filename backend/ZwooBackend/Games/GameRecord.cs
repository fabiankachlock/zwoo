using ZwooGameLogic.Game;

namespace ZwooBackend.Games;

public class GameRecord
{
    Game Game;
    LobbyManager Lobby;

    public GameRecord(Game game, LobbyManager lobby)
    {
        Game = game;
        Lobby = lobby;
    }
}
