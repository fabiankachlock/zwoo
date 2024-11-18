using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Feedback;
using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Settings;

namespace Zwoo.GameEngine.Game.Rules;

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

    public override RuleMeta? Meta => RuleMetaBuilder.New("explicitLastCard")
        .IsTogglable()
        .Default(GameSettingsValue.Off)
        .Localize("de", "Letzte Karte", "Wenn eine Spieler nur noch eine Karte hat, muss er schnell den zwoo Button drücken, sonst erhält er 2 Strafkarten.")
        .Localize("en", "Last card", "If a player has only one card left, he has to press the zwoo button or else he will ge two penalty cards.")
        .ToMeta();

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

    public override GameStateUpdate ApplyInterrupt(GameInterrupt interrupt, GameState state, Pile cardPile, IPlayerCycle playerOrder)
    {
        if (!IsResponsibleForInterrupt(interrupt, state)) return GameStateUpdate.None(state);
        List<GameEvent> events = new();
        List<GameFeedback> feedback = new();

        foreach (var player in interrupt.TargetPlayers)
        {
            _pendingTimeouts.Remove(player);
            List<GameCard> newCards;
            (state, newCards) = DrawCardsForPlayer(state, player, _penaltyCards, cardPile);
            events.Add(GameEvent.SendCards(player, newCards));
            feedback.Add(GameFeedback.Individual(FeedbackType.MissedLast, player));
            feedback.Add(GameFeedback.Individual(FeedbackType.PlayerHasDrawn, player).WithArg(FeedbackArgKey.DrawAmount, newCards.Count));
        }

        return GameStateUpdate.New(state, events, feedback);
    }

    public override bool IsResponsible(ClientEvent gameEvent, GameState state)
    {
        return gameEvent.Type == ClientEventType.RequestEndTurn;
    }

    public override GameStateUpdate ApplyRule(ClientEvent gameEvent, GameState state, Pile cardPile, IPlayerCycle playerOrder)
    {
        if (!IsResponsible(gameEvent, state)) return GameStateUpdate.None(state);
        ClientEvent.RequestEndTurnEvent payload = gameEvent.CastPayload<ClientEvent.RequestEndTurnEvent>();

        if (_pendingTimeouts.ContainsKey(payload.Player))
        {
            _pendingTimeouts[payload.Player].CancellationToken.Cancel();
        }
        return GameStateUpdate.None(state);
    }
}