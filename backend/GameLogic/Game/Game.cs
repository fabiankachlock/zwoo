using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Game;

public sealed class Game
{
    private GameMeta _meta;

    public long Id { get => _meta.Id; }
    public string Name { get => _meta.Name; }
    public bool IsPublic { get => _meta.IsPublic; }
    public bool IsRunning { get => _stateManager.IsRunning; }

    private GameSettings _gameSettings;
    public GameSettings Settings
    {
        get => _gameSettings;
    }

    private PlayerManager _playerManager;
    public List<long> AllPlayers { get => _playerManager.Players; }
    public int PlayerCount { get => _playerManager.PlayerCount; }

    private GameStateManager _stateManager;

    private readonly ILog _logger;

    public Game(
        long id,
        string name,
        bool isPublic
    )
    {
        _meta = new GameMeta();
        _meta.Name = name;
        _meta.Id = id;
        _meta.IsPublic = isPublic;
        _playerManager = new PlayerManager();
        _logger = LogManager.GetLogger($"Game-{id}");
        _gameSettings = GameSettings.FromDefaults();
        _stateManager = new GameStateManager(id, _playerManager, _gameSettings);
    }


    public bool AddPlayer(long id)
    {
        _logger.Debug($"adding player {id}");
        return _playerManager.AddPlayer(id);
    }

    public bool RemovePlayer(long id)
    {
        _logger.Debug($"removing player {id}");
        return _playerManager.RemovePlayer(id);
    }

    public void SetSetting(string key, byte value)
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
    }

    public void HandleEvent<T>(GameEvent<T> gameEvent)
    {
        _logger.Info($"received event: {gameEvent.Type}");
        _stateManager.HandleEvent(gameEvent);
    }

}


