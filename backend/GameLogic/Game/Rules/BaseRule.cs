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
    public abstract int Priority { get; }

    public abstract string Name { get; }

    public abstract GameSettingsKey? AssociatedOption { get; }

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
    /// <summary>
    /// checks if the PlayerId is in the game (valid)
    /// </summary>
    /// <param name="state">game state object</param>
    /// <param name="player">ID of the player</param>
    /// <returns></returns>
    protected bool IsValidPlayer(GameState state, long player)
    {
        bool isValid = state.PlayerDecks.ContainsKey(player);
        if (!isValid)
        {
            _logger.Warn($"player {player} not in game");
        }
        return isValid;
    }

    /// <summary>
    /// checks if the provided PlayerID is the currently active player
    /// </summary>
    /// <param name="state">game state object</param>
    /// <param name="player">ID of the player</param>
    /// <returns></returns>
    protected bool IsActivePlayer(GameState state, long player)
    {
        bool isValid = state.CurrentPlayer == player;
        if (!isValid)
        {
            _logger.Warn($"player {player} not active");
        }
        return IsValidPlayer(state, player) && isValid;
    }

    /// <summary>
    /// checks if a player has a certain card
    /// </summary>
    /// <param name="state">game state object</param>
    /// <param name="player">ID of the player</param>
    /// <param name="card">card the player should have</param>
    /// <returns></returns>
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

    /// <summary>
    /// change the currently active player of the game
    /// </summary>
    /// <param name="state">game state object</param>
    /// <param name="nextPlayer">ID of the next player</param>
    /// <returns>updated state and the created events</returns>
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
