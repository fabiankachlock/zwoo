using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }


    public bool AddPlayer(long id)
    {
        return PlayerManager.AddPlayer(id);
    }

    public bool RemovePlayer(long id)
    {
        return PlayerManager.RemovePlayer(id);
    }

    public void SetSetting(string key, byte value)
    {
        GameSettings.Set(key, value);
    }

    public void Start()
    {

    }

    public void Stop()
    {

    }

    public void Reset()
    {

    }

    // public void HandleEvent<T>(GameEvent<T> gameEvent)
    // {

    // }

}


