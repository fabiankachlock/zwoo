namespace Zwoo.GameEngine.Game.Feedback;

public struct GameFeedback
{
    public FeedbackType Type;
    public FeedbackKind Kind;
    public Dictionary<string, long> Args;

    public GameFeedback(FeedbackType type, FeedbackKind kind, Dictionary<string, long> args)
    {
        Type = type;
        Kind = kind;
        Args = args;
    }

    public static GameFeedback Individual(FeedbackType type, long target)
    {
        return new GameFeedback(type, FeedbackKind.Individual, new Dictionary<string, long> { { FeedbackArgKey.Target, target } });
    }

    public static GameFeedback Interaction(FeedbackType type, long origin, long target)
    {
        return new GameFeedback(type, FeedbackKind.Interaction, new Dictionary<string, long> {
            { FeedbackArgKey.Origin, origin },
            { FeedbackArgKey.Target, target }
        });
    }

    public static GameFeedback Unaffected(FeedbackType type)
    {
        return new GameFeedback(type, FeedbackKind.Unaffected, new Dictionary<string, long>());
    }

    public GameFeedback WithArg(string argKey, long value)
    {
        Args[argKey] = value;
        return this;
    }
}