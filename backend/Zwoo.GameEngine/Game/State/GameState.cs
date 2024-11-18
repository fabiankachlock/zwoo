using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zwoo.GameEngine.Game.Cards;

namespace Zwoo.GameEngine.Game.State;

public struct UiHints
{
    public int? CurrentDrawAmount;
}

public struct GameState
{
    public GameDirection Direction;
    public long CurrentPlayer;
    public List<StackCard> CardStack;
    public StackCard TopCard => CardStack.Last();

    public Dictionary<long, List<GameCard>> PlayerDecks;
    public UiHints Ui;

    public GameState()
    {
        Direction = GameDirection.Left;
        CurrentPlayer = 0;
        CardStack = new List<StackCard>() { };
        PlayerDecks = new Dictionary<long, List<GameCard>>();
        Ui = new UiHints();
    }

    public GameState(
        GameDirection direction,
        long currentPlayer,
        List<StackCard> cardStack,
        Dictionary<long, List<GameCard>> playerDecks,
        UiHints ui
    )
    {
        Direction = direction;
        CurrentPlayer = currentPlayer;
        CardStack = cardStack;
        PlayerDecks = playerDecks;
        Ui = ui;
    }

    public GameState Clone()
    {
        return new GameState(
            Direction,
            CurrentPlayer,
            new List<StackCard>(CardStack),
            PlayerDecks.ToDictionary(kv => kv.Key, kv => new List<GameCard>(kv.Value)),
            Ui
        );
    }
}
