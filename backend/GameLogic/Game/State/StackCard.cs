using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Game.State;

internal struct StackCard
{
    public Card Card;
    public bool EventActivated;

    public StackCard(Card card, bool eventActivated)
    {
        Card = card;
        EventActivated = eventActivated;
    }

    public StackCard(Card card)
    {
        Card = card;
        EventActivated = false;
    }

    public void ActivateEvnt()
    {
        EventActivated = true;
    }
}
