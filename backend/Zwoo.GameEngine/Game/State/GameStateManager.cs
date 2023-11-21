using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Rules;
using Zwoo.GameEngine.Game.Settings;
using Zwoo.GameEngine.Game.Feedback;
using Zwoo.GameEngine.Helper;
using Zwoo.GameEngine.Logging;

namespace Zwoo.GameEngine.Game.State;

public sealed class GameStateManager
{
    public readonly GameMeta Meta;
    private readonly IGameEventManager _notificationManager;
    private readonly RuleManager _ruleManager;
    private PlayerManager _playerManager;
    private GameSettings _gameSettings;
    private GameState _gameState;
    private PlayerCycle _playerCycle;
    private Dictionary<long, int> _playerOrder;
    private Pile _cardPile;
    private AsyncExecutionQueue _actionsQueue;
    private bool _isRunning;

    private ILogger _logger;

    public bool IsRunning
    {
        get => _isRunning;
    }

    public delegate void FinishedHandler(GameEvent.PlayerWonEvent data, GameMeta gameMeta);
    public event FinishedHandler OnFinished = delegate { };

    internal GameStateManager(GameMeta meta, PlayerManager playerManager, GameSettings settings, IGameEventManager notification, ILoggerFactory loggerFactory)
    {
        Meta = meta;
        _gameSettings = settings;
        _playerManager = playerManager;
        _notificationManager = notification;
        _isRunning = false;
        _cardPile = new Pile(_gameSettings);
        _gameState = new GameState();
        _playerCycle = new PlayerCycle(new List<long>());
        _playerOrder = new Dictionary<long, int>();
        _ruleManager = new RuleManager(meta.Id, _gameSettings, loggerFactory);
        _actionsQueue = new AsyncExecutionQueue();
        _logger = loggerFactory.CreateLogger($"GameState-{meta.Id}");
        _playerManager.OnPlayerLeave(HandlePlayerLeave);
    }

    /// <summary>
    /// start a game
    /// </summary>
    internal void Start()
    {
        if (_isRunning)
        {
            _logger.Warn("game already started");
            return;
        }
        _isRunning = true;
        (_playerCycle, _playerOrder) = _playerManager.ComputeOrder();
        _cardPile = new Pile(_gameSettings);
        _actionsQueue.Start();
        _ruleManager.Configure(HandleInterrupt);
        var topCard = _cardPile.DrawSaveCard();
        _gameState = new GameState(
            direction: GameDirection.Left,
            currentPlayer: _playerCycle.ActivePlayer,
            cardStack: new List<StackCard>() { new StackCard(topCard) },
            playerDecks: GeneratePlayerDecks(_playerManager.Players),
            ui: new UiHints()
        );
    }

    /// <summary>
    /// create card decks for every player
    /// </summary>
    /// <param name="players">the list of players to generate decks for</param>
    /// <returns>a list of cards for every player id</returns>
    private Dictionary<long, List<Card>> GeneratePlayerDecks(List<long> players)
    {
        Dictionary<long, List<Card>> decks = new Dictionary<long, List<Card>>();
        foreach (long player in players)
        {
            decks[player] = _cardPile.DrawCard(_gameSettings.NumberOfCards);
        }
        return decks;
    }

    /// <summary>
    /// stop a running game
    /// </summary>
    internal void Stop()
    {
        if (!_isRunning)
        {
            _logger.Warn("game not started");
            return;
        }
        _actionsQueue.Clear();
        _actionsQueue.Stop();
        _isRunning = false;
    }

    /// <summary>
    /// reset a game to its default state
    /// if the game is running it will be stopped
    /// </summary>
    internal void Reset()
    {
        if (_isRunning)
        {
            _logger.Warn("resetting running game");
            Stop();
        }
        _isRunning = false;
        _gameState = new GameState();
        _cardPile = new Pile(_gameSettings);
        _playerCycle = new PlayerCycle(new List<long>());
        _actionsQueue = new AsyncExecutionQueue();
    }

    private async void HandlePlayerLeave(long id)
    {
        await _actionsQueue.Intercept(() =>
        {
            _logger.Warn($"player {id} left the running game");
            long lastPlayer = _playerCycle.ActivePlayer;
            _playerCycle.RemovePlayer(id, _gameState.Direction);
            if (_playerCycle.Order.Count() <= 1) return;

            long newPlayer = _playerCycle.ActivePlayer;

            GameState newState = _gameState.Clone();
            newState.PlayerDecks.Remove(id);
            if (newPlayer != lastPlayer)
            {
                // active player changed --> update game state
                newState.CurrentPlayer = newPlayer;
                List<GameEvent> events = new List<GameEvent>()
                {
                    GameEvent.EndTurn(lastPlayer),
                    GameEvent.StartTurn(newPlayer),
                    GameEvent.CreateStateUpdate(
                        topCard: newState.TopCard.Card,
                        activePlayer: newState.CurrentPlayer,
                        cardAmounts: new Dictionary<long, int>(),
                        feedback: new List<UIFeedback>(),
                        currentDrawAmount: null
                    )
                };
                SendEvents(events);
            }
            _gameState = newState;
        });
    }

    internal void HandleEvent(ClientEvent clientEvent)
    {
        // these calls don't need to be awaited, because the caller doesn't care when the event is executed
        var _ = _actionsQueue.Enqueue(() => ExecuteEvent(clientEvent));
    }

    internal void HandleInterrupt(GameInterrupt interrupt)
    {
        _logger.Debug($"game interrupted scheduled");
        // these calls don't need to be awaited, because the caller doesn't care when the event is executed
        var _ = _actionsQueue.Intercept(() => ExecuteInterrupt(interrupt));
    }

    private void ExecuteEvent(ClientEvent clientEvent)
    {
        GameState newState = _gameState.Clone();
        BaseRule? rule = _ruleManager.GetRule(clientEvent, newState);

        if (rule == null)
        {
            _logger.Error($"cant find rule for event ${clientEvent}");
            return;
        }
        _logger.Debug($"selected rule: {rule.Name}");

        GameStateUpdate stateUpdate = rule.ApplyRule(clientEvent, newState, _cardPile, _playerCycle);
        _postExecute(stateUpdate);
    }

    private void ExecuteInterrupt(GameInterrupt interrupt)
    {
        GameState newState = _gameState.Clone();
        BaseRule? rule = _ruleManager.GetRuleForInterrupt(interrupt, newState);

        if (rule == null)
        {
            _logger.Error($"cant find rule for interrupt ${interrupt}");
            return;
        }
        _logger.Debug($"selected interrupt rule: {rule.Name}");

        GameStateUpdate stateUpdate = rule.ApplyInterrupt(interrupt, newState, _cardPile, _playerCycle);
        _postExecute(stateUpdate);
    }

    private void _postExecute(GameStateUpdate stateUpdate)
    {
        var filteredEvents = stateUpdate.Events.Where(evt => evt.Type != GameEventType.StateUpdate);

        GameEvent stateUpdateEvent = GameEvent.CreateStateUpdate(
            topCard: stateUpdate.NewState.TopCard.Card,
            activePlayer: stateUpdate.NewState.CurrentPlayer,
            cardAmounts: stateUpdate.NewState.PlayerDecks
                .Where(kv => _gameState.PlayerDecks[kv.Key].Count != kv.Value.Count)
                .Select(kv => KeyValuePair.Create(kv.Key, kv.Value.Count))
                .ToDictionary(kv => kv.Key, kv => kv.Value),
            feedback: stateUpdate.Feedback,
            currentDrawAmount: stateUpdate.NewState.Ui.CurrentDrawAmount
         );
        _gameState = stateUpdate.NewState;

        if (!stateUpdate.DiscardExplicitly)
        {
            filteredEvents = filteredEvents.Append(stateUpdateEvent);
        }

        GameEvent? isFinishedEvent = IsGameFinished(_gameState);
        if (isFinishedEvent.HasValue)
        {
            _actionsQueue.Clear();
            Stop();
            SendEvents(new List<GameEvent>() { isFinishedEvent.Value });
            GameEvent.PlayerWonEvent playerWonEvent = isFinishedEvent.Value.CastPayload<GameEvent.PlayerWonEvent>();
            OnFinished.Invoke(playerWonEvent, Meta);
            return;
        }

        SendEvents(filteredEvents.ToList());
        _ruleManager.OnGameUpdate(stateUpdate.NewState, stateUpdate.Events);
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
                    _notificationManager.SendCard(new SendCardDTO(getCardEvent.Player, getCardEvent.Cards));
                    break;
                case GameEventType.RemoveCard:
                    GameEvent.RemoveCardEvent removeCardEvent = evt.CastPayload<GameEvent.RemoveCardEvent>();
                    _notificationManager.RemoveCard(new RemoveCardDTO(removeCardEvent.Player, removeCardEvent.Cards));
                    break;
                case GameEventType.StateUpdate:
                    GameEvent.StateUpdateEvent stateUpdateEvent = evt.CastPayload<GameEvent.StateUpdateEvent>();
                    _notificationManager.StateUpdate(new StateUpdateDTO(
                        stateUpdateEvent.TopCard,
                        stateUpdateEvent.ActivePlayer,
                        stateUpdateEvent.CardAmounts,
                        stateUpdateEvent.Feedback,
                        stateUpdateEvent.CurrentDrawAmount
                    ));
                    break;
                case GameEventType.GetPlayerDecision:
                    GameEvent.PlayerDecisionEvent playerDecisionEvent = evt.CastPayload<GameEvent.PlayerDecisionEvent>();
                    _notificationManager.GetPlayerDecision(new PlayerDecisionDTO(playerDecisionEvent.Player, playerDecisionEvent.Decision, playerDecisionEvent.Options));
                    break;
                case GameEventType.PlayerWon:
                    GameEvent.PlayerWonEvent playerWonEvent = evt.CastPayload<GameEvent.PlayerWonEvent>();
                    _notificationManager.PlayerWon(new PlayerWonDTO(playerWonEvent.Winner, playerWonEvent.Scores), Meta);
                    break;
                case GameEventType.Error:
                    GameEvent.GameErrorEvent errorEvent = evt.CastPayload<GameEvent.GameErrorEvent>();
                    _notificationManager.Error(new ErrorDto(errorEvent.Player, errorEvent.Error, errorEvent.Message));
                    break;
            }
        }
    }

    /* Game State Info Getters */
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

    public int? GetPlayerOrder(long playerId)
    {
        if (_playerOrder.ContainsKey(playerId))
        {
            return _playerOrder[playerId];
        }
        return null;
    }

    public Card GetPileTop()
    {
        return _gameState.TopCard.Card;
    }
}
