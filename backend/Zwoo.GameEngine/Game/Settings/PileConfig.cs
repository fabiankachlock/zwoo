using Zwoo.Api.ZRP;
using Zwoo.GameEngine.Game.Cards;

namespace Zwoo.GameEngine.Game.Settings;

public class PileSettingBuilder : GameSettingBuilder
{
    private PileSettingBuilder(GameCardType key, int defaultAmount = 2) : base(GameSettingsKey.Pile + "." + PileSettings.TypeMapper[key])
    {
        Min = 0;
        Max = 50;
        DefaultValue = defaultAmount;
        Type = SettingsType.Numeric;
    }

    public static PileSettingBuilder New(GameCardType key, int defaultAmount = 2)
    {
        return new PileSettingBuilder(key, defaultAmount);
    }
}

public class PileSettings
{
    public static readonly Dictionary<GameCardType, string> TypeMapper = new Dictionary<GameCardType, string>() {
        { GameCardType.Zero, "zero" },
        { GameCardType.One, "one" },
        { GameCardType.Two, "two" },
        { GameCardType.Three, "three" },
        { GameCardType.Four, "four" },
        { GameCardType.Five, "five" },
        { GameCardType.Six, "six" },
        { GameCardType.Seven, "seven" },
        { GameCardType.Eight, "eight" },
        { GameCardType.Nine, "nine" },
        { GameCardType.Skip, "skip" },
        { GameCardType.Reverse, "reverse" },
        { GameCardType.DrawTwo, "drawTwo" },
        { GameCardType.Wild, "wild" },
        { GameCardType.WildFour, "wildFour" },
    };

    public static string GetKeyForType(GameCardType type)
    {
        return "pile." + TypeMapper[type];
    }

    public static readonly List<GameSetting> Config = new List<GameSetting>()
    {
        GameSettingBuilder.New(GameSettingsKey.Pile).Configure(setting =>
        {
            setting.Localize("de", "Kartenstapel", "Ändere welche Karte zu welcher Anzahl im Kartenstapel vorhanden sind.");
            setting.Localize("en", "Cards pile", "Change whit which amount what cards are in the pile.");
        }).ToSetting(),
        PileSettingBuilder.New(GameCardType.Zero, 2)
            .Localize("de", "Null", "")
            .Localize("en", "Zero", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.One, 2)
            .Localize("de", "Eins", "")
            .Localize("en", "One", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Two, 2)
            .Localize("de", "Zwei", "")
            .Localize("en", "Two", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Three, 2)
            .Localize("de", "Drei", "")
            .Localize("en", "Three", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Four, 2)
            .Localize("de", "Vier", "")
            .Localize("en", "Four", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Five, 2)
            .Localize("de", "Fünf", "")
            .Localize("en", "Five", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Six, 2)
            .Localize("de", "Sechs", "")
            .Localize("en", "Six", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Seven, 2)
            .Localize("de", "Sieben", "")
            .Localize("en", "Seven", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Eight, 2)
            .Localize("de", "Acht", "")
            .Localize("en", "Eight", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Nine, 2)
            .Localize("de", "Neun", "")
            .Localize("en", "Nine", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Skip, 2)
            .Localize("de", "Aussetzer", "")
            .Localize("en", "Skip", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Reverse, 2)
            .Localize("de", "Richtungswechsel", "")
            .Localize("en", "Reverse", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.DrawTwo, 2)
            .Localize("de", "2-Ziehen", "")
            .Localize("en", "Draw two", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.Wild, 4)
            .Localize("de", "Wünscher", "")
            .Localize("en", "Wild", "")
            .ToSetting(),
        PileSettingBuilder.New(GameCardType.WildFour, 4)
            .Localize("de", "Wünscher + 4 Ziehen", "")
            .Localize("en", "Wild draw 4", "")
            .ToSetting(),
    };
}
