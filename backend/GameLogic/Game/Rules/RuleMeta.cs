using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Game.Rules;

public struct RuleMeta
{
    public GameSetting RootSetting;
    public GameSetting[] AllSettings;
}

public class RuleMetaBuilder
{
    public readonly string SettingsKey;
    public GameSettingsType SettingsType;
    public Dictionary<string, string> Title;
    public Dictionary<string, string> Description;
    public int? DefaultValue;
    public int? Min;
    public int? Max;
    public List<GameSettingBuilder> SubSettings;

    private RuleMetaBuilder(string key)
    {
        SettingsKey = key;
        SettingsType = GameSettingsType.Readonly;
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
        SettingsType = GameSettingsType.Boolean;
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