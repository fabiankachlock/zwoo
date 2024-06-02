using Zwoo.GameEngine.Game.Cards;
using Zwoo.GameEngine.Game.Settings;

namespace Zwoo.GameEngine.Tests.Framework;

public class MockPile : Pile
{
    private Pile _fallback = new Pile(GameSettings.FromDefaults());
    private bool _useFallback;
    private List<Card> _programmed;
    private int _currentIdx = 0;

    public MockPile(params Card[] cards) : base(GameSettings.FromDefaults())
    {
        _useFallback = false;
        _programmed = new List<Card>(cards);
    }

    public MockPile(bool useFallback, params Card[] cards) : base(GameSettings.FromDefaults())
    {
        _useFallback = useFallback;
        _programmed = new List<Card>(cards);
    }

    public MockPile(List<Card> cards, bool useFallback) : base(GameSettings.FromDefaults())
    {
        _useFallback = useFallback;
        _programmed = new List<Card>(cards);
    }

    public new Card DrawCard()
    {
        Card card;

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

    public new List<Card> DrawCard(int amount)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            cards.Add(DrawCard());
        }
        return cards;
    }
}