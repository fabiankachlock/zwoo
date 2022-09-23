﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Cards;

namespace ZwooGameLogic.Game.State;

internal class StackCard
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

    public void ActivateEvent()
    {
        EventActivated = true;
    }
}
