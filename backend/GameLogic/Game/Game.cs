using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ZwooGameLogic.Game.Settings;

namespace ZwooGameLogic.Game;

public sealed class Game
{
    private GameMeta Meta;

    public long Id { get => Meta.Id; }
    public string Name { get => Meta.Name; }
    public bool IsPublic { get => Meta.IsPublic; }
    public bool IsRunning { get => Meta.IsRunning; }

    private GameSettings GameSettings;
    public GameSettings Settings
    {
        get => GameSettings;
    }

    private PlayerManager PlayerManager;
    public List<long> AllPlayers { get => PlayerManager.Players; }
    public int PlayerCount { get => PlayerManager.PlayerCount; }

    private readonly ILog _logger;

    public Game(
        long id,
        string name,
        bool isPublic
    )
    {
        Meta = new GameMeta();
        Meta.Name = name;
        Meta.Id = id;
        Meta.IsPublic = isPublic;
        GameSettings = GameSettings.FromDefaults();
        PlayerManager = new PlayerManager();
        _logger = LogManager.GetLogger($"Game-{id}");
    }


    public bool AddPlayer(long id)
    {
        _logger.Debug($"adding player {id}");
        return PlayerManager.AddPlayer(id);
    }

    public bool RemovePlayer(long id)
    {
        _logger.Debug($"removing player {id}");
        return PlayerManager.RemovePlayer(id);
    }

    public void SetSetting(string key, byte value)
    {
        GameSettings.Set(key, value);
    }

    public void Start()
    {
        _logger.Info("starting game");
    }

    public void Stop()
    {
        _logger.Info("stopping game");

    }

    public void Reset()
    {
        _logger.Info("resetting game");

    }

    // public void HandleEvent<T>(GameEvent<T> gameEvent)
    // {

    // }

}


