using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Events;

namespace ZwooGameLogic.Game.State;

internal struct GameStateUpdate
{
    public GameState NewState;
    public List<GameEvent> Events;

    public GameStateUpdate(GameState newState, List<GameEvent> events)
    {
        NewState = newState;
        Events = events;
    }

    public static GameStateUpdate None(GameState state)
    {
        return new GameStateUpdate(state, new List<GameEvent>());
    }

    public static GameStateUpdate WithEvents(GameState state, List<GameEvent> events)
    {
        return new GameStateUpdate(state, events);
    }
}
