using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.Settings;

public struct GameSettings
{
    private Dictionary<GameSettingsKey, int> _settings;

    public int MaxAmountOfPlayers
    {
        get => _settings[GameSettingsKey.MaxAmountOfPlayers];
    }

    public int NumberOfCards
    {
        get => _settings[GameSettingsKey.NumberOfCards];
    }

    private GameSettings(Dictionary<GameSettingsKey, int> initialSettings)
    {
        _settings = initialSettings;
    }

    public static GameSettings FromDefaults()
    {
        Dictionary<GameSettingsKey, int> settings = new Dictionary<GameSettingsKey, int>();
        settings.Add(GameSettingsKey.DEFAULT_RULE_SET, 1);
        settings.Add(GameSettingsKey.MaxAmountOfPlayers, 5);
        settings.Add(GameSettingsKey.NumberOfCards, 7);
        return new GameSettings(settings);
    }

    public static GameSettings Empty()
    {
        return new GameSettings(new Dictionary<GameSettingsKey, int>() { { GameSettingsKey.DEFAULT_RULE_SET, 1 } });
    }

    public bool Set(GameSettingsKey? key, int value)
    {
        if (key.HasValue && _settings.ContainsKey(key.Value!))
        {
            _settings[key.Value!] = value;
            return true;
        }
        return false;
    }

    public bool Set(string stringKey, int value)
    {
        GameSettingsKey? key = SettingsKeyMapper.ToKey(stringKey);
        return Set(key, value);
    }

    public int Get(GameSettingsKey? key)
    {
        if (key.HasValue && _settings.ContainsKey(key.Value!))
        {
            return _settings[key.Value!];
        }
        return 0;
    }

    public int Get(string stringKey)
    {
        GameSettingsKey? key = SettingsKeyMapper.ToKey(stringKey);
        if (key.HasValue)
        {
            return _settings[key.Value];
        }
        return 0;
    }

    // return only enabled settings
    public Dictionary<GameSettingsKey, int> GetSettings()
    {
        return new Dictionary<GameSettingsKey, int>(_settings);
    }
}
