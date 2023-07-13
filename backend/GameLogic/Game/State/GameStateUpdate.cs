using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Events;

namespace ZwooGameLogic.Game.State;

internal struct GameStateUpdate
{
    internal struct GameUpdateEventOverride
    {
        public readonly int? CurrentDrawAmount;
    }


    public GameState NewState;
    public List<GameEvent> Events;
    public GameUpdateEventOverride? OverrideStateUpdate = null;

    public GameStateUpdate(GameState newState, List<GameEvent> events, GameUpdateEventOverride? updateOverride = null)
    {
        NewState = newState;
        Events = events;
        OverrideStateUpdate = updateOverride;
    }

    public static GameStateUpdate None(GameState state)
    {
        return new GameStateUpdate(state, new List<GameEvent>());
    }

    public static GameStateUpdate WithEvents(GameState state, List<GameEvent> events)
    {
        return new GameStateUpdate(state, events);
    }

    public static GameStateUpdate WithOverride(GameState state, List<GameEvent> events, GameUpdateEventOverride updateOverride)
    {
        return new GameStateUpdate(state, events, updateOverride);
    }
}
