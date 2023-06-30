using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.Cards;

internal interface IPile
{
    public Card DrawCard();
    public List<Card> DrawCard(int amount);
}

internal sealed class Pile : IPile
{
    private List<Card> AvailableCards;
    private Random random = new Random();


    public Pile()
    {
        AvailableCards = new List<Card>();
        PopulateStack();
    }

    private List<Card> GetScrambeledCardSet()
    {
        List<Card> cards = new List<Card>();
        for (int color = 1; color <= 4; color++)
        {
            for (int type = 1; type <= 13; type++)
            {
                cards.Add(new Card(color, type));
                cards.Add(new Card(color, type));
            }
        }

        for (int i = 0; i < 4; i++)
        {
            cards.Add(new Card(CardColor.Black, CardType.Wild));
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

}
