using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zwoo.GameEngine.Game.Cards;

namespace Zwoo.GameEngine.Game.State;

public struct StackCard
{
    public GameCard Card;
    public bool EventActivated;

    public StackCard(GameCard card, bool eventActivated)
    {
        Card = card;
        EventActivated = eventActivated;
    }

    public StackCard(GameCard card)
    {
        Card = card;
        EventActivated = false;
    }

    public void ActivateEvent()
    {
        EventActivated = true;
    }

    public override string ToString()
    {
        return $"{Card.Color} {Card.Type} {(EventActivated ? "(activated)" : "(not activated)")}";
    }
}
