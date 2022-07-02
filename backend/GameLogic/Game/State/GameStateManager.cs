using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Cards;
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

    public void HandleEvent(GameEvent gameEvent)
    {

    }
}
