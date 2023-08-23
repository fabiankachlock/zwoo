using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.State;

internal struct UiHints
{
    public int? CurrentDrawAmount;
}

internal struct GameState
{
    public GameDirection Direction;
    public long CurrentPlayer;
    public StackCard TopCard;
    public List<StackCard> CardStack;
    public Dictionary<long, List<Card>> PlayerDecks;
    public UiHints Ui;

    public GameState()
    {
        Direction = GameDirection.Left;
        CurrentPlayer = 0;
        TopCard = new StackCard(new Card(CardColor.None, CardType.None));
        CardStack = new List<StackCard>() { TopCard };
        PlayerDecks = new Dictionary<long, List<Card>>();
        Ui = new UiHints();
    }

    public GameState(
        GameDirection direction,
        long currentPlayer,
        StackCard topCard,
        List<StackCard> cardStack,
        Dictionary<long, List<Card>> playerDecks,
        UiHints ui
    )
    {
        Direction = direction;
        CurrentPlayer = currentPlayer;
        TopCard = topCard;
        CardStack = cardStack;
        PlayerDecks = playerDecks;
        Ui = ui;
    }

    public GameState Clone()
    {
        return new GameState(
            Direction,
            CurrentPlayer,
            new StackCard(TopCard.Card, TopCard.EventActivated),
            new List<StackCard>(CardStack),
            PlayerDecks.ToDictionary(kv => kv.Key, kv => new List<Card>(kv.Value)),
            Ui
        );
    }
}
