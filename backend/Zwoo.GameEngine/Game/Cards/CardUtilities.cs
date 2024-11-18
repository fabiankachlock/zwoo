using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zwoo.GameEngine.Game.Cards;

internal sealed class CardUtilities
{
    public static bool IsDraw(GameCard card)
    {
        return card.Type == GameCardType.DrawTwo || card.Type == GameCardType.WildFour;
    }

    public static bool IsWild(GameCard card)
    {
        return card.Type == GameCardType.Wild || card.Type == GameCardType.WildFour;
    }
}
