using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.Settings;

public struct GameSettings
{
    private Dictionary<GameSettingsKey, byte> Settings;

    private static Dictionary<string, GameSettingsKey> SettingKeys = new Dictionary<string, GameSettingsKey>
    {
        { "maxAmountOfPlayers", GameSettingsKey.MaxAmountOfPlayers },
        { "numberOfCards", GameSettingsKey.NumberOfCards },
    };

    public byte MaxAmountOfPlayers
    {
        get => Settings[GameSettingsKey.MaxAmountOfPlayers];
    }

    public byte NumberOfCards
    {
        get => Settings[GameSettingsKey.NumberOfCards];
    }

    private GameSettings(Dictionary<GameSettingsKey, byte> initialSettings)
    {
        Settings = initialSettings;
    }

    public static GameSettings FromDefaults()
    {
        Dictionary<GameSettingsKey, byte> settings = new Dictionary<GameSettingsKey, byte>();
        settings.Add(GameSettingsKey.MaxAmountOfPlayers, 5);
        settings.Add(GameSettingsKey.NumberOfCards, 7);
        return new GameSettings();
    }

    public static GameSettings Empty()
    {
        return new GameSettings(new Dictionary<GameSettingsKey, byte>());
    }

    public void Set(GameSettingsKey key, byte value)
    {
        if (Settings.ContainsKey(key))
        {
            Settings[key] = value;
        }
    }

    public void Set(string stringKey, byte value)
    {
        if (GameSettings.SettingKeys.ContainsKey(stringKey))
        {
            GameSettingsKey key = GameSettings.SettingKeys[stringKey];
            Set(key, value);
        }
    }
}
