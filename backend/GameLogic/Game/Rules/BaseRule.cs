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

internal abstract class BaseRule
{
    public readonly int Priority = RulePriorirty.None;

    public readonly string Name = "__BaseRule__";

    public readonly GameSettingsKey? AssociatedOption;

    public BaseRule() { }

    public virtual bool IsResponsible(ClientEvent clientEvent, GameState state)
    {
        return false;
    }


    public virtual GameStateUpdate ApplyRule(ClientEvent clientEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        return new GameStateUpdate(state, new List<GameEvent>());
    }

    // Rule utilities
    protected bool PlayerHasCard(GameState state, long player, Card card)
    {
        return state.PlayerDecks[player].Contains(card);
    }
}
