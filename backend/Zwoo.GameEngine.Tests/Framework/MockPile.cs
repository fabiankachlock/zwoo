using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Settings;

namespace Zwoo.GameEngine.Tests.Framework;

public class MockPile : Pile
{
    private Pile _fallback = new Pile(GameSettings.FromDefaults());
    private bool _useFallback;
    private List<GameCard> _programmed;
    private int _currentIdx = 0;

    public MockPile(params GameCard[] cards) : base(GameSettings.FromDefaults())
    {
        _useFallback = false;
        _programmed = new List<GameCard>(cards);
    }

    public MockPile(bool useFallback, params GameCard[] cards) : base(GameSettings.FromDefaults())
    {
        _useFallback = useFallback;
        _programmed = new List<GameCard>(cards);
    }

    public MockPile(List<GameCard> cards, bool useFallback) : base(GameSettings.FromDefaults())
    {
        _useFallback = useFallback;
        _programmed = new List<GameCard>(cards);
    }

    public new GameCard DrawCard()
    {
        GameCard card;

        if (_programmed.Count < _currentIdx)
        {
            card = _programmed[_currentIdx];
            _currentIdx += 1;
        }
        else if (!_useFallback && _programmed.Count > 0)
        {
            _currentIdx = 0;
            card = _programmed[_currentIdx];
        }
        else
        {
            card = _fallback.DrawCard();
        }

        return card;
    }

    public new List<GameCard> DrawCard(int amount)
    {
        List<GameCard> cards = new List<GameCard>();
        for (int i = 0; i < amount; i++)
        {
            cards.Add(DrawCard());
        }
        return cards;
    }
}