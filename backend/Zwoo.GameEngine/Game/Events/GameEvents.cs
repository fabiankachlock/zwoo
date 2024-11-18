using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Feedback;

namespace Zwoo.GameEngine.Game.Events;

public enum GameEventType
{
    StartTurn = 301,
    EndTurn = 302,
    GetCard = 306,
    RemoveCard = 307,
    StateUpdate = 308,
    GetPlayerDecision = 316,
    PlayerWon = 399,
    Error = 400
}

public struct GameEvent
{
    public readonly GameEventType Type;
    public readonly object Payload;

    private GameEvent(GameEventType type, object payload)
    {
        Type = type;
        Payload = payload;
    }

    public T CastPayload<T>()
    {
        return (T)Payload;
    }

    // StartTurnEvent
    public struct StartTurnEvent
    {
        public readonly long Player;

        public StartTurnEvent(long player)
        {
            Player = player;
        }
    }

    public static GameEvent StartTurn(long player)
    {
        return new GameEvent(GameEventType.StartTurn, new StartTurnEvent(player));
    }

    //EndTurnEvent
    public struct EndTurnEvent
    {
        public readonly long Player;

        public EndTurnEvent(long player)
        {
            Player = player;
        }
    }

    public static GameEvent EndTurn(long player)
    {
        return new GameEvent(GameEventType.EndTurn, new EndTurnEvent(player));
    }

    // GetCardEvent
    public struct GetCardEvent
    {
        public readonly long Player;
        public readonly List<GameCard> Cards;

        public GetCardEvent(long player, List<GameCard> cards)
        {
            Player = player;
            Cards = cards;
        }
    }

    public static GameEvent SendCards(long player, List<GameCard> cards)
    {
        return new GameEvent(GameEventType.GetCard, new GetCardEvent(player, cards));
    }

    // RemoveCardEvent
    public struct RemoveCardEvent
    {
        public readonly long Player;
        public readonly List<GameCard> Cards;

        public RemoveCardEvent(long player, List<GameCard> cards)
        {
            Player = player;
            Cards = cards;
        }
    }

    public static GameEvent RemoveCard(long player, List<GameCard> cards)
    {
        return new GameEvent(GameEventType.RemoveCard, new RemoveCardEvent(player, cards));
    }

    // added for compatibility reasons
    public static GameEvent RemoveCard(long player, GameCard card)
    {
        return new GameEvent(GameEventType.RemoveCard, new RemoveCardEvent(player, new List<GameCard>() { card }));
    }


    // StateUpdateEvent
    public struct StateUpdateEvent
    {
        public readonly GameCard TopCard;
        public readonly long ActivePlayer;
        public readonly Dictionary<long, int> CardAmounts;
        public readonly List<GameFeedback> Feedback;
        public readonly int? CurrentDrawAmount;

        public StateUpdateEvent(GameCard topCard, long activePlayer, Dictionary<long, int> cardAmounts, List<GameFeedback> feedback, int? currentDrawAmount)
        {
            TopCard = topCard;
            ActivePlayer = activePlayer;
            CardAmounts = cardAmounts;
            Feedback = feedback;
            CurrentDrawAmount = currentDrawAmount;
        }
    }

    public static GameEvent CreateStateUpdate(GameCard topCard, long activePlayer, Dictionary<long, int> cardAmounts, List<GameFeedback> feedback, int? currentDrawAmount)
    {
        return new GameEvent(GameEventType.StateUpdate, new StateUpdateEvent(topCard, activePlayer, cardAmounts, feedback, currentDrawAmount));
    }

    // PlayerDecisionEvent
    public struct PlayerDecisionEvent
    {
        public readonly long Player;
        public readonly PlayerDecision Decision;
        public readonly List<string> Options;

        public PlayerDecisionEvent(long player, PlayerDecision decision, List<string> options)
        {
            Player = player;
            Decision = decision;
            Options = options;
        }
    }

    public static GameEvent GetPlayerDecision(long player, PlayerDecision decision, List<string> options)
    {
        return new GameEvent(GameEventType.GetPlayerDecision, new PlayerDecisionEvent(player, decision, options));
    }

    // PlayerWonEvent
    public struct PlayerWonEvent
    {
        public readonly long Winner;
        public readonly Dictionary<long, int> Scores;

        public PlayerWonEvent(long winner, Dictionary<long, int> scores)
        {
            Winner = winner;
            Scores = scores;
        }
    }

    public static GameEvent PlayerWon(long player, Dictionary<long, int> scores)
    {
        return new GameEvent(GameEventType.PlayerWon, new PlayerWonEvent(player, scores));
    }

    // PlayerWonEvent
    public struct GameErrorEvent
    {
        public readonly long? Player;
        public readonly GameError Error;
        public readonly string Message;

        public GameErrorEvent(long? player, GameError error, string message)
        {
            Player = player;
            Error = error;
            Message = message;
        }
    }

    // TODO sent message according to error type
    public static GameEvent Error(long? player, GameError error, string message = "")
    {
        return new GameEvent(GameEventType.Error, new GameErrorEvent(player, error, message));
    }

}
