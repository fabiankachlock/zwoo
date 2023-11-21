using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Feedback;

namespace Zwoo.GameEngine.Game.State;

internal struct GameStateUpdate
{

    public GameState NewState;
    public List<GameEvent> Events;
    public List<UIFeedback> Feedback;
    public bool DiscardExplicitly = false;

    private GameStateUpdate(GameState newState, List<GameEvent> events, List<UIFeedback> feedback)
    {
        NewState = newState;
        Events = events;
        Feedback = feedback;
    }

    public static GameStateUpdate New(GameState newState, List<GameEvent> events, List<UIFeedback> feedback)
    {
        return new GameStateUpdate(newState, events, feedback);
    }

    public static GameStateUpdate New(GameState newState, List<GameEvent> events, params UIFeedback[] feedback)
    {
        return new GameStateUpdate(newState, events, feedback.ToList());
    }

    public static GameStateUpdate None(GameState state)
    {
        return new GameStateUpdate(state, new List<GameEvent>(), new List<UIFeedback>())
        {
            DiscardExplicitly = true
        };
    }

    public static GameStateUpdate NoneWithEvents(GameState state, params GameEvent[] events)
    {
        return new GameStateUpdate(state, events.ToList(), new List<UIFeedback>())
        {
            DiscardExplicitly = true
        };
    }

    public static GameStateUpdate NoneWithFeedback(GameState state, params UIFeedback[] feedback)
    {
        return new GameStateUpdate(state, new List<GameEvent>(), feedback.ToList())
        {
            DiscardExplicitly = true
        };
    }

    public static GameStateUpdate WithEvents(GameState state, List<GameEvent> events)
    {
        return new GameStateUpdate(state, events, new List<UIFeedback>());
    }

    public static GameStateUpdate WithEvents(GameState state, params GameEvent[] events)
    {
        return new GameStateUpdate(state, events.ToList(), new List<UIFeedback>());
    }

    public static GameStateUpdate WithFeedback(GameState state, List<UIFeedback> feedback)
    {
        return new GameStateUpdate(state, new List<GameEvent>(), feedback);
    }

    public static GameStateUpdate WithFeedback(GameState state, params UIFeedback[] feedback)
    {
        return new GameStateUpdate(state, new List<GameEvent>(), feedback.ToList());
    }
}
