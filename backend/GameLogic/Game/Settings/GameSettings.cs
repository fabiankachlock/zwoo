using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Game.Settings;

/*
        { "maxPlayers", GameSettingsKey.MaxAmountOfPlayers },
        { "initialCards", GameSettingsKey.NumberOfCards },
        { "addUpDraw", GameSettingsKey.AddUpDraw },
        { "explicitLastCard", GameSettingsKey.ExplicitLastCard }
*/

public static class GameSettingsKey
{
    public static readonly string MaxAmountOfPlayers = "maxPlayers";
    public static readonly string NumberOfCards = "initialCards";
}

public class GameSettings
{
    public static readonly List<GameSetting> BaseSettings = new List<GameSetting>()
    {
        new GameSetting()
        {
            Key = GameSettingsKey.MaxAmountOfPlayers,
            Title = new Dictionary<string, string>(){
                {"de", "DE"},
                {"en", "EN"},
            },
            Description = new Dictionary<string, string>(){
                {"de", "DE"},
                {"en", "EN"},
            },
            Type = GameSettingsType.Numeric,
            Min = 1,
            Max = 20
        },
        new GameSetting()
        {
            Key = GameSettingsKey.NumberOfCards,
            Title = new Dictionary<string, string>(){
                {"de", "DE"},
                {"en", "EN"},
            },
            Description = new Dictionary<string, string>(){
                {"de", "DE"},
                {"en", "EN"},
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
            .Where(r => r.Setting != null)
            .Select(r => new GameSetting()
            {
                Key = r.Setting!.Value.SettingsKey,
                Title = r.Setting.Value.Title,
                Description = r.Setting.Value.Description,
                Type = GameSettingsType.Boolean,
                Value = r.Setting!.Value.DefaultValue ?? 0
            }).ToList();
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
        Dictionary<string, int> settings = new Dictionary<string, int>();
        settings.Add(GameSettingsKey.MaxAmountOfPlayers, 5);
        settings.Add(GameSettingsKey.NumberOfCards, 7);
        foreach (var rule in RuleManager.AllRules())
        {
            if (rule.Setting != null)
            {
                settings.Add(rule.Setting.Value.SettingsKey, rule.Setting.Value.DefaultValue ?? 0);
            }
        }
        return new GameSettings(settings);
    }
}
