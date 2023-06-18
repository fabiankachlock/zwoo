using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.Settings;

public class SettingsKeyMapper
{
    private static Dictionary<string, GameSettingsKey> StringToKey = new Dictionary<string, GameSettingsKey>
    {
        { "maxPlayers", GameSettingsKey.MaxAmountOfPlayers },
        { "initialCards", GameSettingsKey.NumberOfCards },
        { "addUpDraw", GameSettingsKey.AddUpDraw },
        { "explicitLastCard", GameSettingsKey.ExplicitLastCard }
    };

    private static Dictionary<GameSettingsKey, string> KeyToString = new Dictionary<GameSettingsKey, string>
    {
        { GameSettingsKey.MaxAmountOfPlayers, "maxPlayers" },
        { GameSettingsKey.NumberOfCards, "initialCards" },
        { GameSettingsKey.AddUpDraw, "addUpDraw" },
        { GameSettingsKey.ExplicitLastCard, "explicitLastCard" }
    };

    public static string ToString(GameSettingsKey key)
    {
        if (KeyToString.ContainsKey(key))
        {
            return KeyToString[key];
        }
        return "";
    }

    public static GameSettingsKey? ToKey(string key)
    {
        if (StringToKey.ContainsKey(key))
        {
            return StringToKey[key];
        }
        return null;
    }
}
