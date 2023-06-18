using ZwooGameLogic.ZRP;
using ZwooGameLogic.Bots.State;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Logging;


namespace ZwooGameLogic.Bots.Decisions;


public class BasicBotDecisionManager : IBotDecisionHandler
{

    public event IBotDecisionHandler.EventHandler OnEvent = delegate { };

    private BasicBotStateManager _stateManager;
    private int placedCard = -1;
    private ILogger _logger;

    public BasicBotDecisionManager(ILogger logger)
    {
        _stateManager = new BasicBotStateManager();
        _logger = logger;
    }

    public void AggregateNotification(BotZRPNotification<object> message)
    {
        switch (message.Code)
        {
            case ZRPCode.GameStarted:
                OnEvent.Invoke(ZRPCode.GetHand, new GetDeckEvent());
                break;
            case ZRPCode.GetPlayerDecision:
                _logger.Info("making decision");
                makeDecision((GetPlayerDecisionNotification)message.Payload);
                return;
            case ZRPCode.PlaceCardError:
                placeCard();
                return;
            default:
                _stateManager.AggregateNotification((BotZRPNotification<object>)message);
                checkLastCard();
                break;
        }

        var currentState = _stateManager.GetState();
        if (currentState.IsActive && message.Code != ZRPCode.StateUpdated)
        {
            _logger.Info("starting turn");
            placedCard = -1;
            placeCard();
        }
    }

    private void checkLastCard()
    {
        if (_stateManager.GetState().Deck.Count < 2)
        {
            OnEvent.Invoke(ZRPCode.RequestEndTurn, new RequestEndTurnEvent());
        }
    }

    private void placeCard()
    {
        var state = _stateManager.GetState();
        placedCard = placedCard + 1;

        if (placedCard >= state.Deck.Count)
        {
            _logger.Info("bailing with draw");
            OnEvent.Invoke(ZRPCode.DrawCard, new DrawCardEvent());
            return;
        }

        try
        {
            OnEvent.Invoke(ZRPCode.PlaceCard, new PlaceCardEvent(
                (int)state.Deck[placedCard].Color,
                (int)state.Deck[placedCard].Type
            ));
        }
        catch (Exception ex)
        {
            _logger.Error($"cant place card [{placedCard}]: {ex}");
        }
    }

    private void makeDecision(GetPlayerDecisionNotification data)
    {
        switch (data.Type)
        {
            case (int)PlayerDecision.SelectColor:
                // TODO: this should be a subset of the options
                OnEvent.Invoke(ZRPCode.ReceiveDecision, new PlayerDecisionEvent((int)PlayerDecision.SelectColor, (int)CardColorHelper.Random()));
                break;
            default:
                // do at least something
                placeCard();
                break;
        }
    }

    public void Reset()
    {
        _stateManager.Reset();
    }
}