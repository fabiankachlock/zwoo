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
    public List<GameEvent<object>> Events;

    public GameStateUpdate(GameState newState, List<GameEvent<object>> events)
    {
        NewState = newState;
        Events = events;
    }

    public static GameStateUpdate None(GameState state)
    {
        return new GameStateUpdate(state, new List<GameEvent<object>>());
    }
}
