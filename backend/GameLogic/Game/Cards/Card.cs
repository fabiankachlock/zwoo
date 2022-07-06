using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.Cards;

public struct Card
{
    public CardColor Color;
    public CardType Type;

    public Card(CardColor cardColor, CardType cardType)
    {
        Color = cardColor;
        Type = cardType;
    }

    public Card(int cardColor, int cardType)
    {
        Color = (CardColor)cardColor;
        Type = (CardType)cardType;
    }
}
