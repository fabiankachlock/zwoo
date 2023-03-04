using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.Cards;

public enum CardColor
{
    None = 0,
    Red = 1,
    Yellow = 2,
    Blue = 3,
    Green = 4,
    Black = 5
}

public class CardColorHelper
{
    private static Random _random = new Random();

    public static CardColor Random()
    {
        return (CardColor)(_random.Next(4) + 1);
    }
}