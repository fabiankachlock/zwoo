using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.Rules;

internal class LastCardRule : BaseDrawRule
{
    public override int Priority
    {
        get => RulePriority.DefaultRule;
    }

    public override string Name
    {
        get => "LastCardRule";
    }

    public override RuleMeta? Setting => new RuleMeta()
    {
        SettingsKey = "explicitLastCard",
        Title = new Dictionary<string, string>(){
            {"de", "Letzt Karte"},
            {"en", "Last Card"},
        },
        Description = new Dictionary<string, string>(){
            {"de", "Wenn eine Spieler nur noch eine Karte hat, muss er schnell den zwoo Button drücken, sonst erhält er 2 Strafkarten."},
            {"en", "If a player has only one card left, he has to press the zwoo button or else he will ge two penalty cards."},
        },
        DefaultValue = 0,
    };

    public LastCardRule() : base() { }

    private struct Timeout
    {
        public readonly Task WaitingTask;
        public readonly CancellationTokenSource CancellationToken;

        public Timeout(Task waitingTask, CancellationTokenSource cancellationToken)
        {
            WaitingTask = waitingTask;
            CancellationToken = cancellationToken;
        }
    }

    private const string _interruptReason = "lastCardExpire";
    private const int _timeoutMs = 2000;
    private const int _penaltyCards = 2;

    private Dictionary<long, Timeout> _pendingTimeouts = new();

    // check if player has only one card
    // -> check is timeout is already running
    // -> if not set timeout

    // on timeout expire
    // -> interrupt game

    // on interrupt
    // -> reassure if only one card
    // -> send two cards

    public override void OnGameEvent(GameState state, List<GameEvent> outgoingEvents)
    {
        foreach (var (player, deck) in state.PlayerDecks)
        {
            if (deck.Count == 1 && !_pendingTimeouts.ContainsKey(player))
            {
                _logger.Info($"creating timeout for player {player}");
                this._pendingTimeouts[player] = _createTimeout(player);
            }
            else if (deck.Count > 1 && _pendingTimeouts.ContainsKey(player))
            {
                _logger.Info($"cancel timeout for player {player}");
                this._pendingTimeouts[player].CancellationToken.Cancel();
            }
        }
    }

    private Timeout _createTimeout(long player)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        return new Timeout(Task.Run(async () =>
        {
            // TODO: configure waiting amount
            await Task.Delay(_timeoutMs, cts.Token);
            _logger.Info($"timeout expired for {player}");
            if (!cts.IsCancellationRequested)
            {
                InterruptGame(_interruptReason, new InterruptPayload(new List<long>() { player }));
            }
        }), cts);
    }

    public override bool IsResponsibleForInterrupt(GameInterrupt interrupt, GameState state)
    {
        return interrupt.OriginRule == Name && interrupt.Reason == _interruptReason;
    }

    public override GameStateUpdate ApplyInterrupt(GameInterrupt interrupt, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsibleForInterrupt(interrupt, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = new();

        foreach (var player in interrupt.TargetPlayers)
        {
            _pendingTimeouts.Remove(player);
            List<Card> newCards;
            (state, newCards) = DrawCardsForPlayer(state, player, _penaltyCards, cardPile);
            events.Add(GameEvent.SendCards(player, newCards));
        }

        return GameStateUpdate.WithEvents(state, events);
    }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.RequestEndTurn;
    }

    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, PlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = new List<GameEvent>();
        ClientEvent.RequestEndTurnEvent payload = gameEvent.CastPayload<ClientEvent.RequestEndTurnEvent>();

        if (_pendingTimeouts.ContainsKey(payload.Player))
        {
            _pendingTimeouts[payload.Player].CancellationToken.Cancel();
        }
        return GameStateUpdate.None(state);
    }
}