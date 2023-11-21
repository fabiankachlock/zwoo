using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zwoo.GameEngine.Game.Settings;

namespace Zwoo.GameEngine.Game.Cards;

public class EmptyPileException : Exception { }

internal sealed class Pile
{

    private List<Card> AvailableCards;
    private Random random = new Random();
    private IGameSettingsStore _settings;


    public Pile(IGameSettingsStore settings)
    {
        _settings = settings;
        AvailableCards = new List<Card>();
        PopulateStack();
    }

    private List<Card> GetScrambeledCardSet()
    {
        int amount;
        List<Card> cards = new List<Card>();
        for (int color = 1; color <= 4; color++)
        {
            for (int type = 1; type <= 13; type++)
            {
                amount = _settings.Get(PileSettings.GetKeyForType((CardType)type));
                for (int i = 0; i < amount; i++)
                {
                    cards.Add(new Card(color, type));
                }
            }
        }


        amount = _settings.Get(PileSettings.GetKeyForType(CardType.Wild));
        for (int i = 0; i < amount; i++)
        {
            cards.Add(new Card(CardColor.Black, CardType.Wild));
        }

        amount = _settings.Get(PileSettings.GetKeyForType(CardType.WildFour));
        for (int i = 0; i < amount; i++)
        {
            cards.Add(new Card(CardColor.Black, CardType.WildFour));
        }


        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }

        return cards;
    }

    private void PopulateStack()
    {
        AvailableCards = AvailableCards.Concat(GetScrambeledCardSet()).ToList();
    }

    public Card DrawCard()
    {
        if (AvailableCards.Count == 0)
        {
            PopulateStack();
        }
        if (AvailableCards.Count == 0) throw new EmptyPileException();

        Card card = AvailableCards[0];
        AvailableCards.RemoveAt(0);
        return card;
    }

    public List<Card> DrawCard(int amount)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            cards.Add(DrawCard());
        }
        return cards;
    }

    public Card DrawSaveCard()
    {
        if (AvailableCards.Count == 0)
        {
            PopulateStack();
        }
        if (AvailableCards.Count == 0) throw new EmptyPileException();

        int idx = AvailableCards.FindIndex(0, card => card.Color != CardColor.Black && card.Type <= CardType.Nine);
        if (idx == -1)
        {
            // fall back to first card
            idx = 0;
        }
        Card card = AvailableCards[idx];
        AvailableCards.RemoveAt(idx);
        return card;
    }

}
