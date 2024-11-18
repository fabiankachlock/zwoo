using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Settings;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Logging;
using Zwoo.Api.ZRP;

namespace Zwoo.GameEngine.Game.Rules;

public struct RuleMeta
{
    public GameSetting RootSetting;
    public GameSetting[] AllSettings;
    private IGameSettingsStore _currentSettings;

    public void ConfigureWith(IGameSettingsStore settings)
    {
        _currentSettings = settings;
    }

    public int GetParameter(string key)
    {
        return GetParameter(_currentSettings, key);
    }

    public int GetParameter(IGameSettingsStore settings, string key)
    {
        if (RootSetting.Key == key || key.StartsWith(RootSetting.Key)) return _currentSettings.Get(key);
        return _currentSettings.Get(RootSetting.Key + "." + key);
    }
}

public class RuleMetaBuilder
{
    public readonly string SettingsKey;
    public SettingsType SettingsType;
    public Dictionary<string, string> Title;
    public Dictionary<string, string> Description;
    public int? DefaultValue;
    public int? Min;
    public int? Max;
    public List<GameSettingBuilder> SubSettings;

    private RuleMetaBuilder(string key)
    {
        SettingsKey = key;
        SettingsType = SettingsType.Readonly;
        Title = new();
        Description = new();
        DefaultValue = null;
        SubSettings = new();
    }

    public static RuleMetaBuilder New(string settingsKey)
    {
        return new RuleMetaBuilder(settingsKey);
    }

    public RuleMetaBuilder IsTogglable()
    {
        SettingsType = SettingsType.Boolean;
        return this;
    }

    public RuleMetaBuilder Default(int? value)
    {
        DefaultValue = value;
        return this;
    }

    public RuleMetaBuilder Localize(string locale, string title, string description)
    {
        Title[locale] = title;
        Description[locale] = description;
        return this;
    }

    public RuleMetaBuilder Configure(Action<RuleMetaBuilder> handler)
    {
        handler(this);
        return this;
    }

    public RuleMetaBuilder ConfigureParameter(string key, Action<GameSettingBuilder> handler)
    {
        var builder = GameSettingBuilder.New(this.SettingsKey + "." + key);
        SubSettings.Add(builder);
        handler(builder);
        return this;
    }

    public RuleMeta ToMeta()
    {
        var rootSetting = new GameSetting()
        {
            Key = SettingsKey,
            Type = SettingsType,
            Title = Title,
            Description = Description,
            Value = DefaultValue ?? 0,
            Max = Max,
            Min = Min,
        };

        return new RuleMeta()
        {
            RootSetting = rootSetting,
            AllSettings = SubSettings.Select(s => s.ToSetting()).Append(rootSetting).ToArray()
        };
    }
}