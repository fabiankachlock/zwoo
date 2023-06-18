using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.Rules;

internal class RulePriority
{
    internal static readonly int None = -1;
    internal static readonly int BaseRule = 1;
    internal static readonly int DefaultRule = 10_000;
    internal static readonly int GameLogicExtendions = 20_000;
    internal static readonly int CustomRule = 30_000;
    internal static readonly int OverrideAll = 1_000_000;
}
