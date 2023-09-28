using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Settings;

public class PileSettingBuilder : GameSettingBuilder
{
    private PileSettingBuilder(CardType key, int defaultAmount = 2) : base(GameSettingsKey.Pile + "." + PileSettings.TypeMapper[key])
    {
        Min = 0;
        Max = 50;
        DefaultValue = defaultAmount;
        Type = GameSettingsType.Numeric;
    }

    public static PileSettingBuilder New(CardType key, int defaultAmount = 2)
    {
        return new PileSettingBuilder(key, defaultAmount);
    }
}

public class PileSettings
{
    public static readonly Dictionary<CardType, string> TypeMapper = new Dictionary<CardType, string>() {
        { CardType.Zero, "zero" },
        { CardType.One, "one" },
        { CardType.Two, "two" },
        { CardType.Three, "three" },
        { CardType.Four, "four" },
        { CardType.Five, "five" },
        { CardType.Six, "six" },
        { CardType.Seven, "seven" },
        { CardType.Eight, "eight" },
        { CardType.Nine, "nine" },
        { CardType.Skip, "skip" },
        { CardType.Reverse, "reverse" },
        { CardType.DrawTwo, "drawTwo" },
        { CardType.Wild, "wild" },
        { CardType.WildFour, "wildFour" },
    };

    public static string GetKeyForType(CardType type)
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
        PileSettingBuilder.New(CardType.Zero, 2)
            .Localize("de", "Null", "")
            .Localize("en", "Zero", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.One, 2)
            .Localize("de", "Eins", "")
            .Localize("en", "One", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Two, 50)
            .Localize("de", "Zwei", "")
            .Localize("en", "Two", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Three, 2)
            .Localize("de", "Drei", "")
            .Localize("en", "Three", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Four, 2)
            .Localize("de", "Vier", "")
            .Localize("en", "Four", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Five, 2)
            .Localize("de", "Fünf", "")
            .Localize("en", "Five", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Six, 2)
            .Localize("de", "Sechs", "")
            .Localize("en", "Six", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Seven, 2)
            .Localize("de", "Sieben", "")
            .Localize("en", "Seven", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Eight, 2)
            .Localize("de", "Acht", "")
            .Localize("en", "Eight", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Nine, 2)
            .Localize("de", "Neun", "")
            .Localize("en", "Nine", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Skip, 2)
            .Localize("de", "Aussetzer", "")
            .Localize("en", "Skip", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Reverse, 2)
            .Localize("de", "Richtungswechsel", "")
            .Localize("en", "Reverse", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.DrawTwo, 2)
            .Localize("de", "2-Ziehen", "")
            .Localize("en", "Draw two", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.Wild, 4)
            .Localize("de", "Wünscher", "")
            .Localize("en", "Wild", "")
            .ToSetting(),
        PileSettingBuilder.New(CardType.WildFour, 4)
            .Localize("de", "Wünscher + 4 Ziehen", "")
            .Localize("en", "Wild draw 4", "")
            .ToSetting(),
    };
}
