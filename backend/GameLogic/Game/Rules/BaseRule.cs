﻿using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Game.Rules;

internal abstract class BaseRule
{
    public abstract int Priority { get; }

    public abstract string Name { get; }

    public abstract GameSettingsKey? AssociatedOption { get; }

    protected ILogger _logger;
    private Action<GameInterrupt> _interrupt;

    private void _voidInterrupt(GameInterrupt data) { }

    public BaseRule(Action<GameInterrupt>? interruptHandler = null, ILogger? logger = null)
    {
        _logger = logger ?? new VoidLogger();
        _interrupt = interruptHandler ?? _voidInterrupt;
    }

    internal void SetupRule(Action<GameInterrupt>? interruptHandler, ILogger? logger)
    {
        _logger = logger ?? _logger;
        _interrupt = interruptHandler ?? _interrupt;
    }

    //Base Rule Stuff
    public virtual bool IsResponsible(ClientEvent clientEvent, GameState state)
    {
        return false;
    }

    public virtual GameStateUpdate ApplyRule(ClientEvent clientEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        return GameStateUpdate.None(state);
    }

    // listening
    public virtual void OnGameEvent(GameState state, List<GameEvent> outgoingEvents) { }

    // Interrupts
    protected void InterruptGame(string reason, InterruptPayload payload)
    {
        _interrupt.Invoke(new GameInterrupt(Name, reason, payload.TargetPlayers));
    }

    public virtual bool IsResponsibleForInterrupt(GameInterrupt clientEvent, GameState state)
    {
        return false;
    }

    public virtual GameStateUpdate ApplyInterrupt(GameInterrupt interrupt, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        return GameStateUpdate.None(state);
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
