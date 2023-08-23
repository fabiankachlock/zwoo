using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Game.Rules;

public struct RuleMeta
{
    public string SettingsKey;
    public Dictionary<string, string> Title;
    public Dictionary<string, string> Description;
    public int? DefaultValue;

}