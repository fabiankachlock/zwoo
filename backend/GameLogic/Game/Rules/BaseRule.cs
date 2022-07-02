using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Rules;

internal class BaseRule
{
    public readonly int Priority = -1;

    public readonly string Name = "__BaseRule__";

    public readonly GameSettingsKey? AssociatedOption;

    public BaseRule() { }

    public bool isResponsible(GameEvent<object> gameEvent, GameState state)
    {
        return false;
    }


    public GameStateUpdate applyRule(GameEvent<object> gameEvent, GameState state, Pile cardPile)
    {
        return new GameStateUpdate(state, new List<object>());
    }
}
