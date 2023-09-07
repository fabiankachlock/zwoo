using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Game.Settings;

public static class GameSettingsKey
{
    public static readonly string MaxAmountOfPlayers = "maxPlayers";
    public static readonly string NumberOfCards = "initialCards";
}

public class GameSettings : IGameSettingsStore
{
    public static readonly List<GameSetting> BaseSettings = new List<GameSetting>()
    {
        new GameSetting()
        {
            Key = GameSettingsKey.MaxAmountOfPlayers,
            Title = new Dictionary<string, string>(){
                {"de", "Spieleranzahl"},
                {"en", "Amount of Players"},
            },
            Description = new Dictionary<string, string>(){
                {"de", "Lege eine maximale Anzahl an Mitspielern für dein Spiel fest."},
                {"en", "Change maximum amount of players for your game."},
            },
            Type = GameSettingsType.Numeric,
            Min = 1,
            Max = 20
        },
        new GameSetting()
        {
            Key = GameSettingsKey.NumberOfCards,
            Title = new Dictionary<string, string>(){
                {"de", "Anzahl an Karten"},
                {"en", "Amount of Cards"},
            },
            Description = new Dictionary<string, string>(){
                {"de", "Lege die Anzahl an Karten fest, die ein Spieler am Anfang bekommt."},
                {"en", "Choose the amount of cards each player is supplied with at the start of a game."},
            },
            Type = GameSettingsType.Numeric,
            Min = 1,
            Max = 30
        }
    };

    private Dictionary<string, int> _settingValues;

    public int MaxAmountOfPlayers
    {
        get => _settingValues[GameSettingsKey.MaxAmountOfPlayers];
    }

    public int NumberOfCards
    {
        get => _settingValues[GameSettingsKey.NumberOfCards];
    }

    private GameSettings(Dictionary<string, int> initialSettings)
    {
        _settingValues = initialSettings;
    }

    public bool Set(string key, int value)
    {
        if (_settingValues.ContainsKey(key))
        {
            _settingValues[key] = value;
            return true;
        }
        return false;
    }

    public int Get(string key)
    {
        if (_settingValues.ContainsKey(key))
        {
            return _settingValues[key];
        }
        return 0;
    }

    private static List<GameSetting> GetFromRules()
    {
        return RuleManager.AllRules()
            .Where(r => r.Meta != null)
            .SelectMany(r => r.Meta!.Value.AllSettings).ToList();
    }

    public List<GameSetting> GetSettings()
    {
        return BaseSettings.ToList().Concat(GetFromRules()).Select(s =>
        {
            // add actual value
            s.Value = _settingValues[s.Key];
            return s;
        }).ToList();
    }

    public static GameSettings FromDefaults()
    {
        Dictionary<string, int> settings = new Dictionary<string, int>
        {
            { GameSettingsKey.MaxAmountOfPlayers, 5 },
            { GameSettingsKey.NumberOfCards, 7 }
        };

        foreach (var rule in RuleManager.AllRules())
        {
            if (rule.Meta != null)
            {
                foreach (var setting in rule.Meta.Value.AllSettings)
                {
                    settings.Add(setting.Key, setting.Value);
                }
            }
        }
        return new GameSettings(settings);
    }
}
