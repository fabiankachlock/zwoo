using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.Settings;

public struct GameSettings
{
    private Dictionary<GameSettingsKey, int> Settings;

    private static Dictionary<string, GameSettingsKey> SettingKeys = new Dictionary<string, GameSettingsKey>
    {
        { "maxAmountOfPlayers", GameSettingsKey.MaxAmountOfPlayers },
        { "numberOfCards", GameSettingsKey.NumberOfCards },
    };

    public int MaxAmountOfPlayers
    {
        get => Settings[GameSettingsKey.MaxAmountOfPlayers];
    }

    public int NumberOfCards
    {
        get => Settings[GameSettingsKey.NumberOfCards];
    }

    private GameSettings(Dictionary<GameSettingsKey, int> initialSettings)
    {
        Settings = initialSettings;
    }

    public static GameSettings FromDefaults()
    {
        Dictionary<GameSettingsKey, int> settings = new Dictionary<GameSettingsKey, int>();
        settings.Add(GameSettingsKey.MaxAmountOfPlayers, 5);
        settings.Add(GameSettingsKey.NumberOfCards, 7);
        return new GameSettings();
    }

    public static GameSettings Empty()
    {
        return new GameSettings(new Dictionary<GameSettingsKey, int>());
    }

    public void Set(GameSettingsKey? key, int value)
    {
        if (key.HasValue && Settings.ContainsKey(key.Value!))
        {
            Settings[key.Value!] = value;
        }
    }

    public void Set(string stringKey, int value)
    {
        if (GameSettings.SettingKeys.ContainsKey(stringKey))
        {
            GameSettingsKey key = GameSettings.SettingKeys[stringKey];
            Set(key, value);
        }
    }

    public int Get(GameSettingsKey? key)
    {
        if (key.HasValue && Settings.ContainsKey(key.Value!))
        {
            return Settings[key.Value!];
        }
        return 0;
    }

    public int Get(string stringKey)
    {
        if (GameSettings.SettingKeys.ContainsKey(stringKey))
        {
            GameSettingsKey key = GameSettings.SettingKeys[stringKey];
            return Settings[key];
        }
        return 0;
    }
}
