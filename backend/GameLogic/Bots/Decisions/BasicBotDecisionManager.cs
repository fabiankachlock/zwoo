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
    private Random _rand { get; set; } = new();

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
                _stateManager.AggregateNotification(message);
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
            if (state.Deck.Count == 2 && _rand.Next(10) > 4)
            {
                Console.WriteLine("BOT ENDING TURN");
                // after placing this card only on card will be left + 50% chance to miss
                OnEvent.Invoke(ZRPCode.RequestEndTurn, new RequestEndTurnEvent());
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"cant place card [{placedCard}]: {ex}");
        }
    }

    private void makeDecision(GetPlayerDecisionNotification data)
    {
        var decision = _rand.Next(data.Options.Count);
        OnEvent.Invoke(ZRPCode.ReceiveDecision, new PlayerDecisionEvent(data.Type, decision));
    }

    public void Reset()
    {
        _stateManager.Reset();
    }
}