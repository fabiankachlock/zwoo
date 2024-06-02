using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zwoo.GameEngine.Game.Cards;

internal sealed class CardUtilities
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
