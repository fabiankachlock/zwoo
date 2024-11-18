namespace Zwoo.GameEngine.Game.Cards;

public struct GameCard
{
    public GameCardColor Color = GameCardColor.None;
    public GameCardType Type = GameCardType.None;

    public GameCard(GameCardColor cardColor, GameCardType cardType)
    {
        Color = cardColor;
        Type = cardType;
    }

    public GameCard(int cardColor, int cardType)
    {
        Color = (GameCardColor)cardColor;
        Type = (GameCardType)cardType;
    }
}
