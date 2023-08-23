using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Game.Settings;

public struct GameSetting
{
    public string Key;
    public Dictionary<string, string> Title;
    public Dictionary<string, string> Description;

    public GameSettingsType Type;
    public int Value;
    public int? Min;
    public int? Max;
}