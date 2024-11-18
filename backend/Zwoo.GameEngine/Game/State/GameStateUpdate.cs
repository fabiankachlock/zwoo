using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Feedback;

namespace Zwoo.GameEngine.Game.State;

public struct GameStateUpdate
{

    public GameState NewState;
    public List<GameEvent> Events;
    public List<GameFeedback> Feedback;
    public bool DiscardExplicitly = false;

    private GameStateUpdate(GameState newState, List<GameEvent> events, List<GameFeedback> feedback)
    {
        NewState = newState;
        Events = events;
        Feedback = feedback;
    }

    public static GameStateUpdate New(GameState newState, List<GameEvent> events, List<GameFeedback> feedback)
    {
        return new GameStateUpdate(newState, events, feedback);
    }

    public static GameStateUpdate New(GameState newState, List<GameEvent> events, params GameFeedback[] feedback)
    {
        return new GameStateUpdate(newState, events, feedback.ToList());
    }

    public static GameStateUpdate None(GameState state)
    {
        return new GameStateUpdate(state, new List<GameEvent>(), new List<GameFeedback>())
        {
            DiscardExplicitly = true
        };
    }

    public static GameStateUpdate NoneWithEvents(GameState state, params GameEvent[] events)
    {
        return new GameStateUpdate(state, events.ToList(), new List<GameFeedback>())
        {
            DiscardExplicitly = true
        };
    }

    public static GameStateUpdate NoneWithFeedback(GameState state, params GameFeedback[] feedback)
    {
        return new GameStateUpdate(state, new List<GameEvent>(), feedback.ToList())
        {
            DiscardExplicitly = true
        };
    }

    public static GameStateUpdate WithEvents(GameState state, List<GameEvent> events)
    {
        return new GameStateUpdate(state, events, new List<GameFeedback>());
    }

    public static GameStateUpdate WithEvents(GameState state, params GameEvent[] events)
    {
        return new GameStateUpdate(state, events.ToList(), new List<GameFeedback>());
    }

    public static GameStateUpdate WithFeedback(GameState state, List<GameFeedback> feedback)
    {
        return new GameStateUpdate(state, new List<GameEvent>(), feedback);
    }

    public static GameStateUpdate WithFeedback(GameState state, params GameFeedback[] feedback)
    {
        return new GameStateUpdate(state, new List<GameEvent>(), feedback.ToList());
    }
}
