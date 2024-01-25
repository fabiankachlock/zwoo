using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Settings;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Logging;

namespace Zwoo.GameEngine.Game.Settings;

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