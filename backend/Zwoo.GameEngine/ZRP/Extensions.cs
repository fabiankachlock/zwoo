using Zwoo.Api.ZRP;
using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Feedback;

namespace Zwoo.GameEngine.ZRP;

public static class CardColorExtensions
{
    public static CardColor ToZRP(this GameCardColor cardColor)
    {
        return cardColor switch
        {
            GameCardColor.Red => CardColor.Red,
            GameCardColor.Yellow => CardColor.Yellow,
            GameCardColor.Blue => CardColor.Blue,
            GameCardColor.Green => CardColor.Green,
            GameCardColor.Black => CardColor.Black,
            _ => CardColor.None
        };
    }


    public static GameCardColor ToGame(this CardColor cardColor)
    {
        return cardColor switch
        {
            CardColor.Red => GameCardColor.Red,
            CardColor.Yellow => GameCardColor.Yellow,
            CardColor.Blue => GameCardColor.Blue,
            CardColor.Green => GameCardColor.Green,
            CardColor.Black => GameCardColor.Black,
            _ => GameCardColor.None
        };
    }
}

public static class CardTypeExtensions
{
    public static CardType ToZRP(this GameCardType cardType)
    {
        return cardType switch
        {
            GameCardType.None => CardType.None,
            GameCardType.Zero => CardType.Zero,
            GameCardType.One => CardType.One,
            GameCardType.Two => CardType.Two,
            GameCardType.Three => CardType.Three,
            GameCardType.Four => CardType.Four,
            GameCardType.Five => CardType.Five,
            GameCardType.Six => CardType.Six,
            GameCardType.Seven => CardType.Seven,
            GameCardType.Eight => CardType.Eight,
            GameCardType.Nine => CardType.Nine,
            GameCardType.Skip => CardType.Skip,
            GameCardType.Reverse => CardType.Reverse,
            GameCardType.DrawTwo => CardType.DrawTwo,
            GameCardType.Wild => CardType.Wild,
            GameCardType.WildFour => CardType.WildFour,
            _ => CardType.None
        };
    }

    public static GameCardType ToGame(this CardType cardType)
    {
        return cardType switch
        {
            CardType.None => GameCardType.None,
            CardType.Zero => GameCardType.Zero,
            CardType.One => GameCardType.One,
            CardType.Two => GameCardType.Two,
            CardType.Three => GameCardType.Three,
            CardType.Four => GameCardType.Four,
            CardType.Five => GameCardType.Five,
            CardType.Six => GameCardType.Six,
            CardType.Seven => GameCardType.Seven,
            CardType.Eight => GameCardType.Eight,
            CardType.Nine => GameCardType.Nine,
            CardType.Skip => GameCardType.Skip,
            CardType.Reverse => GameCardType.Reverse,
            CardType.DrawTwo => GameCardType.DrawTwo,
            CardType.Wild => GameCardType.Wild,
            CardType.WildFour => GameCardType.WildFour,
            _ => GameCardType.None
        };
    }
}

public static class GameCardExtensions
{
    public static Card ToZRP(this GameCard card)
    {
        return new Card(card.Color.ToZRP(), card.Type.ToZRP());
    }

    public static GameCard ToGame(this Card card)
    {
        return new GameCard(card.Color.ToGame(), card.Type.ToGame());
    }
}

public static class FeedbackTypeExtension
{
    public static UIFeedbackType ToZRP(this FeedbackType feedbackType)
    {
        return feedbackType switch
        {
            FeedbackType.DeckSwapped => UIFeedbackType.DeckSwapped,
            FeedbackType.PlayerHasDrawn => UIFeedbackType.PlayerHasDrawn,
            FeedbackType.DirectionChanged => UIFeedbackType.DirectionChanged,
            FeedbackType.MissedLast => UIFeedbackType.MissedLast,
            FeedbackType.ColorChanged => UIFeedbackType.ColorChanged,
            FeedbackType.Skipped => UIFeedbackType.Skipped,
        };
    }
}

public static class FeedbackKindExtension
{
    public static UIFeedbackKind ToZRP(this FeedbackKind feedbackKind)
    {
        return feedbackKind switch
        {
            FeedbackKind.Interaction => UIFeedbackKind.Interaction,
            FeedbackKind.Individual => UIFeedbackKind.Individual,
            FeedbackKind.Unaffected => UIFeedbackKind.Unaffected,
        };
    }
}

public static class ZRPCardExtensions
{
    public static GameCard ToGameCard(this Card card)
    {
        return new GameCard(card.Color.ToGame(), card.Type.ToGame());
    }
}