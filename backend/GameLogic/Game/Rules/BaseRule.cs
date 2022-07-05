using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;
using log4net;

namespace ZwooGameLogic.Game.Rules;

internal abstract class BaseRule
{
    public readonly int Priority = RulePriorirty.None;

    public readonly string Name = "__BaseRule__";

    public readonly GameSettingsKey? AssociatedOption;

    protected ILog _logger;

    public BaseRule(ILog? logger = null)
    {
        _logger = logger ?? LogManager.GetLogger(Name);
    }

    internal void SetLogger(ILog? logger)
    {
        _logger = logger ?? _logger;
    }

    public virtual bool IsResponsible(ClientEvent clientEvent, GameState state)
    {
        return false;
    }


    public virtual GameStateUpdate ApplyRule(ClientEvent clientEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        return new GameStateUpdate(state, new List<GameEvent>());
    }

    // Rule utilities
    protected bool IsValidPlayer(GameState state, long player)
    {
        bool isValid = state.PlayerDecks.ContainsKey(player);
        if (!isValid)
        {
            _logger.Warn($"player {player} not in game");
        }
        return isValid;
    }

    protected bool IsActivePlayer(GameState state, long player)
    {
        bool isValid = state.CurrentPlayer == player;
        if (!isValid)
        {
            _logger.Warn($"player {player} not active");
        }
        return IsValidPlayer(state, player) && isValid;
    }

    protected bool PlayerHasCard(GameState state, long player, Card card)
    {
        if (!IsValidPlayer(state, player)) return false;
        bool isValid = state.PlayerDecks[player].Contains(card);
        if (!isValid)
        {
            _logger.Warn($"player {player} does not has card {card}");
        }
        return isValid;
    }

    protected (GameState, List<GameEvent>) ChangeActivePlayer(GameState state, long nextPlayer)
    {
        List<GameEvent> events = new List<GameEvent>() {
            GameEvent.EndTurn(state.CurrentPlayer),
            GameEvent.StartTurn(nextPlayer),
        };
        state.CurrentPlayer = nextPlayer;
        return (state, events);
    }
}
