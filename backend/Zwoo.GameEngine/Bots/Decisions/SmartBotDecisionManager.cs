using Zwoo.Api.ZRP;
using Zwoo.GameEngine.Bots.State;
using Zwoo.GameEngine.Logging;
using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.ZRP;


namespace Zwoo.GameEngine.Bots.Decisions;


public class SmartBotDecisionManager : IBotDecisionHandler
{

    public event IBotDecisionHandler.EventHandler OnEvent = delegate { };

    private WholeGameBotStateManager _stateManager;
    private int placedCard = -1;
    private ILogger _logger;
    private Random _rand { get; set; } = new();

    public SmartBotDecisionManager(ILogger logger)
    {
        _stateManager = new WholeGameBotStateManager();
        _logger = logger;
    }

    public void AggregateNotification(BotZRPNotification<object> message)
    {
        switch (message.Code)
        {
            case ZRPCode.GameStarted:
                OnEvent.Invoke(ZRPCode.GetDeck, new GetDeckEvent());
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
                state.Deck[placedCard].ToZRP()
            ));
            if (state.Deck.Count == 2 && _rand.Next(10) > 4)
            {
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

        if (data.Type == (int)PlayerDecision.SelectColor)
        {
            decideColor(data);
        }
        else if (data.Type == (int)PlayerDecision.SelectPlayer)
        {
            decidePlayer(data);
        }
        var decision = _rand.Next(data.Options.Count);
        OnEvent.Invoke(ZRPCode.SendPlayerDecision, new PlayerDecisionEvent(data.Type, decision));
    }

    private void decideColor(GetPlayerDecisionNotification data)
    {
        List<GameCardColor> options = [];
        _stateManager.GetState().Deck.ForEach(card =>
        {
            if (card.Type != GameCardType.Wild && card.Type != GameCardType.WildFour)
            {
                options.Add(card.Color);
            }
        });

        if (options.Count == 0)
        {
            options = [
                GameCardColor.Red,
                GameCardColor.Yellow,
                GameCardColor.Blue,
                GameCardColor.Green
            ];
        }

        var decision = options[_rand.Next(options.Count)];
        OnEvent.Invoke(ZRPCode.SendPlayerDecision, new PlayerDecisionEvent(data.Type, data.Options.IndexOf(((int)decision).ToString())));
    }

    private void decidePlayer(GetPlayerDecisionNotification data)
    {
        var players = _stateManager.GetOtherPlayers();
        var lowestAmount = data.Options
            .Select(id => KeyValuePair.Create(id, players[long.Parse(id)]))
            .OrderBy(kv => kv.Value)
            .FirstOrDefault();

        if (lowestAmount.Key == null)
        {
            OnEvent.Invoke(ZRPCode.SendPlayerDecision, new PlayerDecisionEvent(data.Type, _rand.Next(data.Options.Count)));
            return;
        }

        OnEvent.Invoke(ZRPCode.SendPlayerDecision, new PlayerDecisionEvent(data.Type, data.Options.IndexOf(lowestAmount.Key)));
    }

    public void Reset()
    {
        _stateManager.Reset();
    }
}