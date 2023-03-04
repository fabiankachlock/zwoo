using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Game;

public sealed class Game
{
    private GameMeta _meta;
    private GameSettings _gameSettings;
    private PlayerManager _playerManager;
    private GameStateManager _stateManager;
    private readonly IGameEventManager _notificationManager;
    private readonly ILogger _logger;

    public long Id { get => _meta.Id; }
    public string Name { get => _meta.Name; }
    public bool IsPublic { get => _meta.IsPublic; }
    public bool IsRunning { get => _stateManager.IsRunning; }

    public GameSettings Settings
    {
        get => _gameSettings;
    }

    public List<long> AllPlayers { get => _playerManager.Players; }
    public int PlayerCount { get => _playerManager.PlayerCount; }

    public event GameStateManager.FinishedHandler OnFinished
    {
        add => _stateManager.OnFinished += value;
        remove => _stateManager.OnFinished -= value;
    }

    public GameStateManager State
    {
        get => _stateManager;
    }

    public Game(
        long id,
        string name,
        bool isPublic,
        IGameEventManager notificationManager,
        ILoggerFactory loggerFactory
    )
    {
        _meta = new GameMeta();
        _meta.Name = name;
        _meta.Id = id;
        _meta.IsPublic = isPublic;
        _notificationManager = notificationManager;
        _gameSettings = GameSettings.FromDefaults();
        _playerManager = new PlayerManager();
        _stateManager = new GameStateManager(_meta, _playerManager, _gameSettings, _notificationManager, loggerFactory);
        _logger = loggerFactory.CreateLogger($"Game-{id}");
    }


    public bool AddPlayer(long id)
    {
        _logger.Debug($"adding player {id}");
        return _playerManager.AddPlayer(id);
    }

    public bool HasPlayer(long id)
    {
        return _playerManager.HasPlayer(id);
    }

    public bool RemovePlayer(long id)
    {
        _logger.Debug($"removing player {id}");
        bool result = _playerManager.RemovePlayer(id);

        if (PlayerCount <= 1)
        {
            Stop();
        }
        return result;
    }

    public void SetSetting(string key, int value)
    {
        _gameSettings.Set(key, value);
    }

    public void Start()
    {
        _logger.Info("starting game");
        _stateManager.Start();
    }

    public void Stop()
    {
        _logger.Info("stopping game");
        _stateManager.Stop();
    }

    public void Reset()
    {
        _logger.Info("resetting game");
        _stateManager.Reset();
        _playerManager.Reset();
    }

    public void HandleEvent(ClientEvent clientEvent)
    {
        if (IsRunning)
        {
            _logger.Info($"received event: {clientEvent.Type}");
            _stateManager.HandleEvent(clientEvent);
        }
    }
}


