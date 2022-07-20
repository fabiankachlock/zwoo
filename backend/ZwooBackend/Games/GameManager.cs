using ZwooGameLogic.Game;

namespace ZwooBackend.Games;

public class GameManager
{
    public readonly Websockets.WebSocketManager WebSocketManager;

    private readonly ZwooGameLogic.GameManager _gameManager;

    private readonly Dictionary<long, GameRecord> _games;

    public static GameManager Global = new GameManager();

    private GameManager()
    {
        WebSocketManager = new Websockets.WebSocketManager();
        _gameManager = new ZwooGameLogic.GameManager(id => WebSocketManager.GetNotificationManager(id));
        _games = new Dictionary<long, GameRecord>();
    }


    public bool HasGame(long id)
    {
        return this._games.ContainsKey(id);
    }

    public long CreateGame(string name, bool isPublic)
    {
        // TODO: implement password
        Game game = _gameManager.CreateGame(name, isPublic);
        _games[game.Id] = new GameRecord(game, new LobbyManager(game.Id));
        return game.Id;
    }

    public void RemoveGame()
    {

    }

    public GameRecord? GetGame(long id)
    {
        if (_games.ContainsKey(id))
        {
            return _games[id];
        }
        return null;
    }


}
