using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.State;

internal struct GameState
{
    public GameDirection Direction;
    public long CurrentPlayer;
    public Card TopCard;
    public Stack<Card> CardStack;
    public Dictionary<long, List<Card>> PlayerDecks;

    public GameState()
    {
        Direction = GameDirection.Left;
        CurrentPlayer = 0;
        TopCard = new Card(CardColor.None, CardType.None);
        CardStack = new Stack<Card>();
        PlayerDecks = new Dictionary<long, List<Card>>();
    }
}
