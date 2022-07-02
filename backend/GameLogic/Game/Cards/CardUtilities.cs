using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.Cards;

internal class CardUtilities
{
    public static bool IsDraw(Card card)
    {
        return card.Type == CardType.DrawTwo || card.Type == CardType.WildFour;
    }

    public static bool IsWild(Card card)
    {
        return card.Type == CardType.Wild || card.Type == CardType.WildFour;
    }
}
