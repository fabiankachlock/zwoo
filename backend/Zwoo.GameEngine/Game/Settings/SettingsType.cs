using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zwoo.GameEngine.Game.Settings;

public enum GameSettingsType
{
    Readonly = 0,
    Numeric = 1,
    Boolean = 2
}

public sealed class GameSettingsValue
{
    public static int On = 1;
    public static int Off = 0;
}