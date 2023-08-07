namespace ZwooGameLogic.Game.Feedback;

public struct UIFeedback
{
    public UIFeedbackType Type;
    public UIFeedbackKind Kind;
    public Dictionary<string, long> Args;

    public UIFeedback(UIFeedbackType type, UIFeedbackKind kind, Dictionary<string, long> args)
    {
        Type = type;
        Kind = kind;
        Args = args;
    }

    public static UIFeedback Individual(UIFeedbackType type, long target)
    {
        return new UIFeedback(type, UIFeedbackKind.Individual, new Dictionary<string, long> { { UIFeedbackArgKey.Target, target } });
    }

    public static UIFeedback Interaction(UIFeedbackType type, long origin, long target)
    {
        return new UIFeedback(type, UIFeedbackKind.Interaction, new Dictionary<string, long> {
            { UIFeedbackArgKey.Origin, origin },
            { UIFeedbackArgKey.Target, target }
        });
    }

    public static UIFeedback Unaffected(UIFeedbackType type)
    {
        return new UIFeedback(type, UIFeedbackKind.Unaffected, new Dictionary<string, long>());
    }
}