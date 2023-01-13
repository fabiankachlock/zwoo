using ZwooGameLogic.ZRP;
using ZwooGameLogic.Bots.State;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Cards;


namespace ZwooGameLogic.Bots.Decisions;


public class BasicBotDecisionManager : IBotDecisionHandler
{
    private BasicBotStateManager _stateManager;
    private int placedCard = -1;

    public BasicBotDecisionManager()
    {
        _stateManager = new BasicBotStateManager();
    }

    public BotZRPNotification<object>? AggregateNotification(BotZRPNotification<object> message)
    {
        switch (message.Code)
        {
            case ZRPCode.GetPlayerDecision:
                return makeDecision((GetPlayerDecisionDTO)message.Payload);
            case ZRPCode.PlaceCardError:
                return placeCard();
            default:
                _stateManager.AggregateNotification((BotZRPNotification<object>)message);
                break;
        }

        var currentState = _stateManager.GetState();
        if (currentState.IsActive)
        {
            placedCard = -1;
            return placeCard();
        }

        return null;
    }

    private BotZRPNotification<object> placeCard()
    {
        if (placedCard == _stateManager.GetState().Deck.Count)
        {
            return new BotZRPNotification<object>(ZRPCode.DrawCard, new DrawCardDTO());
        }

        placedCard = placedCard + 1;
        var state = _stateManager.GetState();
        return new BotZRPNotification<object>(ZRPCode.PlaceCard, new PlaceCardDTO(
            (int)state.Deck[placedCard].Color,
            (int)state.Deck[placedCard].Type
        ));
    }

    private BotZRPNotification<object> makeDecision(GetPlayerDecisionDTO data)
    {
        switch (data.Type)
        {
            case (int)PlayerDecision.SelectColor:
                return new BotZRPNotification<object>(ZRPCode.ReceiveDecision, new ReceiveDecisionDTO((int)PlayerDecision.SelectColor, (int)CardColorHelper.Random()));
            default:
                // do at least something
                return placeCard();
        }
    }

    public void Reset()
    {
        _stateManager.Reset();
    }
}