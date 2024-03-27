using Zwoo.GameEngine.Game.State;
using Zwoo.GameEngine.Game.Events;
using Zwoo.GameEngine.Game.Rules;
using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game;

namespace Zwoo.GameEngine.Tests.Framework;

internal class GameScenario
{
    internal static readonly int DefaultPlayer = 0;

    private string _name;
    private GameState _state;
    private Pile _pile = new MockPile();
    private PlayerCycle _players = new MockPlayerCycle();
    private List<BaseRule> _rules = new List<BaseRule>();

    private BaseRule? _selectedRule = null;
    private GameStateUpdate? _output = null;

    private GameScenario(string name)
    {
        _name = name;
        _state = new GameState()
        {
            Direction = GameDirection.Left,
            CardStack = new List<StackCard>(),
            CurrentPlayer = 0,
            PlayerDecks = new Dictionary<long, List<Card>> { { 0, new List<Card>() } },
        };
    }

    public static GameScenario Create(string name)
    {
        return new GameScenario(name);
    }

    public GameScenario WithPile(Pile cardPile)
    {
        _pile = cardPile;
        return this;
    }

    public GameScenario WithPileWithCards(params Card[] cards)
    {
        _pile = new MockPile(false, cards);
        return this;
    }

    public GameScenario WithPlayerCycle(PlayerCycle playerCycle)
    {
        _players = playerCycle;
        return this;
    }

    public GameScenario WithPlayers(List<long> players)
    {
        _players = new MockPlayerCycle(players);
        return this;
    }

    public GameScenario WithPlayersAndCards(Dictionary<long, List<Card>> playersAndCards)
    {
        _players = new MockPlayerCycle(playersAndCards.Keys.ToList());
        _state.PlayerDecks = playersAndCards;
        return this;
    }

    public GameScenario WithDeck(params Card[] cards)
    {
        Dictionary<long, List<Card>> playersAndCards = new() { { 0, new List<Card>(cards) } };
        _players = new MockPlayerCycle(playersAndCards.Keys.ToList());
        _state.PlayerDecks = playersAndCards;
        return this;
    }

    public GameScenario WithState(GameState state)
    {
        _state = state;
        return this;
    }

    public GameScenario WithStack(List<StackCard> stack)
    {
        _state.CardStack = stack;
        return this;
    }

    public GameScenario WithDirection(GameDirection direction)
    {
        _state.Direction = direction;
        return this;
    }

    public GameScenario WithTopCard(StackCard topCard)
    {
        _state.CardStack.Add(topCard);
        return this;
    }

    public GameScenario WithTopCard(CardColor c, CardType t)
    {
        StackCard topCard = new StackCard(new Card(c, t), false);
        _state.CardStack.Add(topCard);
        return this;
    }

    public GameScenario WithActivePlayer(long player)
    {
        _state.CurrentPlayer = player;
        return this;
    }

    public GameScenario WithRules(List<BaseRule> rules)
    {
        _rules = rules;
        return this;
    }

    public GameScenario WithRule(BaseRule rule)
    {
        _rules = new List<BaseRule>() { rule };
        return this;
    }

    public GameScenario Trigger(ClientEvent clientEvent)
    {
        Assert.Greater(_rules.Count, 0, $"{_name} scenario has rules");

        _selectedRule = _rules.Where(rule => rule.IsResponsible(clientEvent, _state)).OrderByDescending(rule => rule.Priority).FirstOrDefault();
        if (_selectedRule != null)
        {
            _output = _selectedRule.ApplyRule(clientEvent, _state, _pile, _players);
        }

        return this;
    }

    public GameScenario ExpectSelectedRule(string name)
    {
        Assert.That(name, Is.EqualTo(_selectedRule?.Name), "");
        return this;
    }

    public GameScenario ExpectEvent(GameEvent publishedEvent)
    {
        Assert.IsTrue(_output?.Events.Contains(publishedEvent));
        return this;
    }

    public GameScenario ExpectEventsLike(Func<List<GameEvent>, bool> comparator)
    {
        Assert.IsTrue(comparator(_output?.Events ?? new List<GameEvent>()));
        return this;
    }

    public GameScenario ExpectState(GameState newState)
    {
        Assert.That(newState, Is.EqualTo(_output?.NewState));
        return this;
    }

    public GameScenario ExpectStateLike(Func<GameState, bool> comparator)
    {
        Assert.IsTrue(comparator(_output?.NewState ?? new GameState()));
        return this;
    }

    public GameScenario ExpectActivePlayer(long playerId)
    {
        Assert.That(playerId, Is.EqualTo(_output?.NewState.CurrentPlayer));
        return this;
    }

    public GameScenario ExpectTopCard(StackCard card)
    {
        Assert.That(card, Is.EqualTo(_output?.NewState.TopCard));
        return this;
    }


    public GameScenario WaitForInterrupt(GameInterrupt interrupt)
    {
        // TODO
        return this;
    }

    public GameScenario WaitForInterruptLike(Func<GameInterrupt, bool> comparator)
    {
        // TODO
        return this;
    }

    public GameScenario ShouldTriggerRule(BaseRule targetRule, ClientEvent clientEvent)
    {
        WithRule(targetRule).Trigger(clientEvent).ExpectSelectedRule(targetRule.Name);
        return Reset();
    }

    public GameScenario ShouldNotTriggerRule(BaseRule targetRule, ClientEvent clientEvent)
    {
        WithRule(targetRule).Trigger(clientEvent);
        Assert.IsNull(_selectedRule);
        return Reset();
    }

    public GameScenario Reset()
    {
        _selectedRule = null;
        _output = null;
        return this;
    }
}
