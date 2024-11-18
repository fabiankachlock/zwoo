using Zwoo.Api.ZRP;

namespace Zwoo.GameEngine.Game.Settings;

public class GameSettingBuilder
{
    public readonly string Key;
    public Dictionary<string, string> Title;
    public Dictionary<string, string> Description;

    public SettingsType Type;
    public int? DefaultValue;
    public int? Min;
    public int? Max;

    protected GameSettingBuilder(string key)
    {
        Key = key;
        Title = new();
        Description = new();
        Type = SettingsType.Readonly;
    }

    public static GameSettingBuilder New(string key) => new GameSettingBuilder(key);

    public GameSettingBuilder Localize(string locale, string title, string description)
    {
        Title[locale] = title;
        Description[locale] = description;
        return this;
    }

    public GameSettingBuilder Configure(Action<GameSettingBuilder> handler)
    {
        handler(this);
        return this;
    }

    public GameSetting ToSetting() => new GameSetting()
    {
        Key = Key,
        Title = Title,
        Description = Description,
        Type = Type,
        Value = DefaultValue ?? 0,
        Min = Min,
        Max = Max
    };
}