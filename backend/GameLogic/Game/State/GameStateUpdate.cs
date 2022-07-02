using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.State;

internal struct GameStateUpdate
{
    public GameState NewState;
    public List<object> Events;

    public GameStateUpdate(GameState newState, List<object> events)
    {
        NewState = newState;
        Events = events;
    }
}
