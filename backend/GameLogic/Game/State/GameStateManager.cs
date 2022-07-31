using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Cards;
using ZwooGameLogic.Game.Rules;
using ZwooGameLogic.Game.Settings;

namespace ZwooGameLogic.Game.State;

public sealed class GameStateManager
{
    public readonly long GameId;
    private readonly NotificationManager _notificationManager;
    private readonly RuleManager _ruleManager;
    private PlayerManager _playerManager;
    private GameSettings _gameSettings;
    private GameState _gameState;
    private PlayerCycle _playerCycle;
    private Pile _cardPile;
    private ConcurrentQueue<ClientEvent> _events;
    private bool _isExecutingEvent;
    private bool _isRunning;

    private ILog _logger;

    public bool IsRunning
    {
        get => _isRunning;
    }

    internal GameStateManager(long id, PlayerManager playerManager, GameSettings settings, NotificationManager notification)
    {
        GameId = id;
        _gameSettings = settings;
        _playerManager = playerManager;
        _notificationManager = notification;
        _isRunning = false;
        _isExecutingEvent = false;
        _cardPile = new Pile();
        _gameState = new GameState();
        _playerCycle = new PlayerCycle(new List<long>());
        _ruleManager = new RuleManager(id, _gameSettings);
        _events = new ConcurrentQueue<ClientEvent>();
        _logger = LogManager.GetLogger($"GameState-{id}");
    }


    internal void Start()
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
            topCard: new StackCard(_cardPile.DrawCard()),
            cardStack: new List<StackCard>(),
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

    internal void Stop()
    {
        if (!_isRunning)
        {
            _logger.Warn("game not started");
            return;
        }
        _isRunning = false;
    }

    internal void Reset()
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

    public long ActivePlayer()
    {
        return _gameState.CurrentPlayer;
    }

    public List<Card>? GetPlayerDeck(long playerId)
    {
        if (_gameState.PlayerDecks.ContainsKey(playerId))
        {
            return _gameState.PlayerDecks[playerId];
        }
        return null;
    }

    public int? GetPlayerCardAmount(long playerId)
    {
        if (_gameState.PlayerDecks.ContainsKey(playerId))
        {
            return _gameState.PlayerDecks[playerId].Count;
        }
        return null;
    }

    public Card GetPileTop()
    {
        return _gameState.TopCard.Card;
    }

    internal void HandleEvent(ClientEvent clientEvent)
    {
        _events.Enqueue(clientEvent);
        TryExecuteEvent();
    }

    private void TryExecuteEvent()
    {
        if (!_isExecutingEvent && _events.Count > 0)
        {
            ClientEvent evt;
            if (_events.TryDequeue(out evt))
            {
                ExecuteEvent(evt);
            }
        }
    }

    private void ExecuteEvent(ClientEvent clientEvent)
    {
        _isExecutingEvent = true;
        GameState newState = _gameState.Clone();
        BaseRule? rule = _ruleManager.getRule(clientEvent, newState);

        if (rule == null)
        {
            _logger.Error($"cant find rule for event ${clientEvent}");
            _isExecutingEvent = false;
            TryExecuteEvent();
            return;
        }
        _logger.Debug($"selected rule: {rule.Name}");

        GameStateUpdate stateUpdate = rule.ApplyRule(clientEvent, _gameState, _cardPile, _playerCycle);
        GameEvent stateUpdateEvent = GameEvent.CreateStateUpdate(
            topCard: stateUpdate.NewState.TopCard.Card,
            activePlayer: stateUpdate.NewState.CurrentPlayer,
            activePlayerCardAmount: stateUpdate.NewState.PlayerDecks[stateUpdate.NewState.CurrentPlayer].Count,
            lastPlayer: _gameState.CurrentPlayer,
            lastPlayerCardAmount: stateUpdate.NewState.PlayerDecks[_gameState.CurrentPlayer].Count
         );
        _gameState = stateUpdate.NewState;

        GameEvent? isFinishedEvent = IsGameFinished(_gameState);

        if (isFinishedEvent.HasValue)
        {
            SendEvents(new List<GameEvent>() { isFinishedEvent.Value });
            // reset queue
            _isExecutingEvent = false;
            _events.Clear();
            Stop();
        }
        else
        {
            SendEvents(stateUpdate.Events.Where(evt => evt.Type != GameEventType.StateUpdate).Append(stateUpdateEvent).ToList());
            // handle next event
            _isExecutingEvent = false;
            TryExecuteEvent();
        }
    }

    private GameEvent? IsGameFinished(GameState state)
    {
        foreach (KeyValuePair<long, List<Card>> entry in state.PlayerDecks)
        {
            if (entry.Value.Count() == 0)
            {
                // has winner
                return GameEvent.PlayerWon(entry.Key, state.PlayerDecks.Select(entry => new KeyValuePair<long, int>(entry.Key, entry.Value.Count())).ToDictionary(entry => entry.Key, entry => entry.Value));
            }
        }
        return null;
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
                case GameEventType.Error:
                    GameEvent.GameErrorEvent errorEvent = evt.CastPayload<GameEvent.GameErrorEvent>();
                    _notificationManager.Error(new ErrorDto(errorEvent.Player, errorEvent.Error, errorEvent.Message));
                    break;
            }
        }
    }
}
