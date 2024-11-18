using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zwoo.GameEngine.Game.Cards;

public enum GameCardColor
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

    public static GameCardColor Random()
    {
        return (GameCardColor)(_random.Next(4));
    }
}