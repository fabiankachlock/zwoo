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
        return _games.ContainsKey(id);
    }

    public long CreateGame(string name, bool isPublic)
    {
        Game game = _gameManager.CreateGame(name, isPublic);
        _games[game.Id] = new GameRecord(game, new LobbyManager(game.Id, game.Settings));
        return game.Id;
    }

    public void RemoveGame(long id)
    {
        if (HasGame(id))
        {
            var record = _games[id];
            _gameManager.RemoveGame(record.Game.Id);
            _games.Remove(id);
        }
    }

    public GameRecord? GetGame(long id)
    {
        if (_games.ContainsKey(id))
        {
            return _games[id];
        }
        return null;
    }

    public List<GameRecord> ListAll()
    {
        return _games.Values.ToList();
    }

}
