using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Game.Rules;
using ZwooGameLogic.Game.Settings;

namespace ZwooGameLogic.Game.State;

internal class GameStateManager
{
    public readonly long GameId;
    private readonly NotificationManager _notificationManager;
    private readonly RuleManager _ruleManager;
    private PlayerManager _playerManager;
    private GameSettings _gameSettings;
    private GameState _gameState;
    private PlayerCycle _playerCycle;
    private Pile _cardPile;
    private bool _isRunning;

    private ILog _logger;

    public bool IsRunning
    {
        get => _isRunning;
    }

    public GameStateManager(long id, PlayerManager playerManager, GameSettings settings, NotificationManager notification)
    {
        GameId = id;
        _gameSettings = settings;
        _playerManager = playerManager;
        _notificationManager = notification;
        _isRunning = false;
        _cardPile = new Pile();
        _gameState = new GameState();
        _playerCycle = new PlayerCycle(new List<long>());
        _ruleManager = new RuleManager(_gameSettings);
        _logger = LogManager.GetLogger($"GameState-{id}");
    }


    public void Start()
    {
        if (_isRunning)
        {
            _logger.Warn("game already started");
            return;
        }
        _isRunning = true;
        _playerCycle = _playerManager.ComputeOrder();
        _cardPile = new Pile();
        _ruleManager.Configure();
        _gameState = new GameState(
            direction: GameDirection.Left,
            currentPlayer: _playerCycle.ActivePlayer,
            topCard: _cardPile.DrawCard(),
            cardStack: new List<Card>(),
            playerDecks: GeneratePlayerDecks()
        );
    }

    private Dictionary<long, List<Card>> GeneratePlayerDecks()
    {
        Dictionary<long, List<Card>> decks = new Dictionary<long, List<Card>>();
        foreach (long player in _playerManager.Players)
        {
            decks[player] = _cardPile.DrawCard(_gameSettings.NumberOfCards);
        }
        return decks;
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            _logger.Warn("game not started");
            return;
        }
        _isRunning = false;
    }

    public void Reset()
    {
        if (!_isRunning)
        {
            _logger.Warn("resetting running game");
        }
        _isRunning = false;
        _gameState = new GameState();
        _cardPile = new Pile();
        _playerCycle = new PlayerCycle(new List<long>());
    }

    public void HandleEvent(ClientEvent clientEvent)
    {
        // TODO: queue events
        GameState newState = _gameState.Clone();
        BaseRule? rule = _ruleManager.getRule(clientEvent, newState);

        if (rule == null)
        {
            _logger.Error($"cant find rule for event ${clientEvent}");
            return;
        }
        _logger.Debug($"selected rule: {rule.Name}");

        GameStateUpdate stateUpdate = rule.ApplyRule(clientEvent, _gameState, _cardPile, _playerCycle);
        GameEvent stateUpdateEvent = GameEvent.CreateStateUpdate(
            topCard: stateUpdate.NewState.TopCard,
            activePlayer: stateUpdate.NewState.CurrentPlayer,
            activePlayerCardAmount: stateUpdate.NewState.PlayerDecks[stateUpdate.NewState.CurrentPlayer].Count,
            lastPlayer: _gameState.CurrentPlayer,
            lastPlayerCardAmount: stateUpdate.NewState.PlayerDecks[_gameState.CurrentPlayer].Count
         );
        _gameState = stateUpdate.NewState;

        SendEvents(stateUpdate.Events.Where(evt => evt.Type != GameEventType.StateUpdate).Append(stateUpdateEvent).ToList());
    }

    private void SendEvents(List<GameEvent> events)
    {
        foreach (GameEvent evt in events)
        {
            switch (evt.Type)
            {
                case GameEventType.StartTurn:
                    GameEvent.StartTurnEvent startTurnEvent = evt.CastPayload<GameEvent.StartTurnEvent>();
                    _notificationManager.StartTurn(startTurnEvent.Player);
                    break;
                case GameEventType.EndTurn:
                    GameEvent.EndTurnEvent endTurnEvent = evt.CastPayload<GameEvent.EndTurnEvent>();
                    _notificationManager.EndTurn(endTurnEvent.Player);
                    break;
                case GameEventType.GetCard:
                    GameEvent.GetCardEvent getCardEvent = evt.CastPayload<GameEvent.GetCardEvent>();
                    _notificationManager.SendCard(new SendCardDTO(getCardEvent.Player, getCardEvent.Card));
                    break;
                case GameEventType.RemoveCard:
                    GameEvent.RemoveCardEvent removeCardEvent = evt.CastPayload<GameEvent.RemoveCardEvent>();
                    _notificationManager.RemoveCard(new RemoveCardDTO(removeCardEvent.Player, removeCardEvent.Card));
                    break;
                case GameEventType.StateUpdate:
                    GameEvent.StateUpdateEvent stateUpdateEvent = evt.CastPayload<GameEvent.StateUpdateEvent>();
                    _notificationManager.StateUpdate(new StateUpdateDTO(
                        stateUpdateEvent.TopCard,
                        stateUpdateEvent.ActivePlayer,
                        stateUpdateEvent.ActivePlayerCardAmount,
                        stateUpdateEvent.LastPlayer,
                        stateUpdateEvent.LastPlayerCardAmount
                    ));
                    break;
                case GameEventType.GetPlayerDecission:
                    GameEvent.PlayerDecissionEvent playerDecissionEvent = evt.CastPayload<GameEvent.PlayerDecissionEvent>();
                    _notificationManager.GetPlayerDecission(new PlayerDecissionDTO(playerDecissionEvent.Player, playerDecissionEvent.Decission));
                    break;
                case GameEventType.PlayerWon:
                    GameEvent.PlayerWonEvent playerWonEvent = evt.CastPayload<GameEvent.PlayerWonEvent>();
                    _notificationManager.PlayerWon(new PlayerWonDTO(playerWonEvent.Winner, playerWonEvent.Scores));
                    break;
            }
        }
    }
}
