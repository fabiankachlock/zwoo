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
using ZwooGameLogic.Helper;

namespace ZwooGameLogic.Game.State;

public sealed class GameStateManager
{
    public readonly GameMeta Meta;
    private readonly NotificationManager _notificationManager;
    private readonly RuleManager _ruleManager;
    private PlayerManager _playerManager;
    private GameSettings _gameSettings;
    private GameState _gameState;
    private PlayerCycle _playerCycle;
    private Dictionary<long, int> _playerOrder;
    private Pile _cardPile;
    private AsyncExecutionQueue _actionsQueue;
    private bool _isRunning;

    private ILog _logger;

    public bool IsRunning
    {
        get => _isRunning;
    }

    internal GameStateManager(GameMeta meta, PlayerManager playerManager, GameSettings settings, NotificationManager notification)
    {
        Meta = meta;
        _gameSettings = settings;
        _playerManager = playerManager;
        _notificationManager = notification;
        _isRunning = false;
        _cardPile = new Pile();
        _gameState = new GameState();
        _playerCycle = new PlayerCycle(new List<long>());
        _playerOrder = new Dictionary<long, int>();
        _ruleManager = new RuleManager(meta.Id, _gameSettings);
        _actionsQueue = new AsyncExecutionQueue();
        _logger = LogManager.GetLogger($"GameState-{Meta.Id}");
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
        _cardPile = new Pile();
        _actionsQueue.Start();
        _ruleManager.Configure();
        _gameState = new GameState(
            direction: GameDirection.Left,
            currentPlayer: _playerCycle.ActivePlayer,
            topCard: new StackCard(DrawSaveCard()),
            cardStack: new List<StackCard>(),
            playerDecks: GeneratePlayerDecks(_playerManager.Players)
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
        _isRunning = false;
    }

    /// <summary>
    /// reset a game to its default state
    /// if the game is running it will be stopped
    /// </summary>
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

    private void HandlePlayerLeave(long id)
    {
        #pragma warning disable CS4014
        _actionsQueue.Intercept(() =>
        {
            _logger.Warn($"player {id} left the running game");
            long lastPlayer = _playerCycle.ActivePlayer;
            _playerCycle.RemovePlayer(id);
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
                        activePlayerCardAmount: newState.PlayerDecks[newState.CurrentPlayer].Count,
                        lastPlayer: _gameState.CurrentPlayer,
                        lastPlayerCardAmount: _gameState.CurrentPlayer == id ? 0 : newState.PlayerDecks[_gameState.CurrentPlayer].Count
                    )
                };
                Console.WriteLine(events);
                SendEvents(events);
            }
            _gameState = newState;
        });
        #pragma warning restore CS4014
    }

    internal void HandleEvent(ClientEvent clientEvent)
    {
        // these calls don't need to be awaited, because the caller doesn't care when the event is executed
        #pragma warning disable CS4014
        _actionsQueue.Enqueue(() => ExecuteEvent(clientEvent));
        #pragma warning restore CS4014
    }

    private void ExecuteEvent(ClientEvent clientEvent)
    {
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
            _actionsQueue.Clear();
            Stop();
        }
        else
        {
            SendEvents(stateUpdate.Events.Where(evt => evt.Type != GameEventType.StateUpdate).Append(stateUpdateEvent).ToList());
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
                    _notificationManager.PlayerWon(new PlayerWonDTO(playerWonEvent.Winner, playerWonEvent.Scores), Meta);
                    break;
                case GameEventType.Error:
                    GameEvent.GameErrorEvent errorEvent = evt.CastPayload<GameEvent.GameErrorEvent>();
                    _notificationManager.Error(new ErrorDto(errorEvent.Player, errorEvent.Error, errorEvent.Message));
                    break;
            }
        }
    }

    private Card DrawSaveCard()
    {
        while (true)
        {
            Card card = _cardPile.DrawCard();
            if (card.Color != CardColor.Black && card.Type <= CardType.Nine)
            {
                return card;
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
