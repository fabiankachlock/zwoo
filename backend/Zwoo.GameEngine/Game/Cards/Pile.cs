using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zwoo.GameEngine.Game.Settings;

namespace Zwoo.GameEngine.Game.Cards;

public class EmptyPileException : Exception { }

public class Pile
{

    private List<GameCard> AvailableCards;
    private Random random = new Random();
    private IGameSettingsStore _settings;


    public Pile(IGameSettingsStore settings)
    {
        _settings = settings;
        AvailableCards = new List<GameCard>();
        PopulateStack();
    }

    private List<GameCard> GetScrambeledCardSet()
    {
        int amount;
        List<GameCard> cards = new List<GameCard>();
        for (int color = 1; color <= 4; color++)
        {
            for (int type = 1; type <= 13; type++)
            {
                amount = _settings.Get(PileSettings.GetKeyForType((GameCardType)type));
                for (int i = 0; i < amount; i++)
                {
                    cards.Add(new GameCard(color, type));
                }
            }
        }


        amount = _settings.Get(PileSettings.GetKeyForType(GameCardType.Wild));
        for (int i = 0; i < amount; i++)
        {
            cards.Add(new GameCard(GameCardColor.Black, GameCardType.Wild));
        }

        amount = _settings.Get(PileSettings.GetKeyForType(GameCardType.WildFour));
        for (int i = 0; i < amount; i++)
        {
            cards.Add(new GameCard(GameCardColor.Black, GameCardType.WildFour));
        }


        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            GameCard value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }

        return cards;
    }

    private void PopulateStack()
    {
        AvailableCards = AvailableCards.Concat(GetScrambeledCardSet()).ToList();
    }

    public GameCard DrawCard()
    {
        if (AvailableCards.Count == 0)
        {
            PopulateStack();
        }
        if (AvailableCards.Count == 0) throw new EmptyPileException();

        GameCard card = AvailableCards[0];
        AvailableCards.RemoveAt(0);
        return card;
    }

    public List<GameCard> DrawCard(int amount)
    {
        List<GameCard> cards = new List<GameCard>();
        for (int i = 0; i < amount; i++)
        {
            cards.Add(DrawCard());
        }
        return cards;
    }

    public GameCard DrawSaveCard()
    {
        if (AvailableCards.Count == 0)
        {
            PopulateStack();
        }
        if (AvailableCards.Count == 0) throw new EmptyPileException();

        int idx = AvailableCards.FindIndex(0, card => card.Color != GameCardColor.Black && card.Type <= GameCardType.Nine);
        if (idx == -1)
        {
            // fall back to first card
            idx = 0;
        }
        GameCard card = AvailableCards[idx];
        AvailableCards.RemoveAt(idx);
        return card;
    }

}
