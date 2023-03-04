import { defineStore } from 'pinia';
import { ref } from 'vue';

import { ZwooConfigKey } from '@/core/adapter/config';
import { useGameEventDispatch } from '@/core/adapter/play/util/useGameEventDispatch';
import { Card } from '@/core/services/game/CardTypes';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';

import { CardDeck } from '../../services/game/PlayerDeck';
import { useConfig } from '../config';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

const deckWatcher = new MonolithicEventWatcher(ZRPOPCode.GameStarted, ZRPOPCode.GetCards, ZRPOPCode.RemoveCard, ZRPOPCode.GetHand);

export const useGameCardDeck = defineStore('game-cards', () => {
  const cards = ref<Card[]>([]);
  const selectedCard = ref<(Card & { index: number }) | undefined>(undefined);
  let deck = new CardDeck([]);
  const dispatchEvent = useGameEventDispatch();
  const config = useConfig();

  const _receiveMessage: typeof deckWatcher['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.GameStarted) {
      dispatchEvent(ZRPOPCode.RequestHand, {});
    } else if (msg.code === ZRPOPCode.GetCards) {
      for (const card of msg.data.cards) {
        addCard({
          color: card.type,
          type: card.symbol
        });
      }
    } else if (msg.code === ZRPOPCode.RemoveCard) {
      removeCard({
        color: msg.data.type,
        type: msg.data.symbol
      });
    } else if (msg.code === ZRPOPCode.GetHand) {
      setState(
        msg.data.hand.map(c => ({
          color: c.type,
          type: c.symbol
        }))
      );
    }
  };

  const setState = (newCards: Card[]) => {
    const config = useConfig();
    deck = new CardDeck(newCards);
    if (config.get(ZwooConfigKey.SortCards)) {
      cards.value = deck.sorted;
      return;
    }
    cards.value = deck.cards;
  };

  const addCard = (card: Card) => {
    const config = useConfig();
    deck.pushCard(card);
    if (config.get(ZwooConfigKey.SortCards)) {
      cards.value = deck.sorted;
      return;
    }
    cards.value = deck.cards;
  };

  const hasNext = (direction: 'before' | 'after'): boolean => {
    const nextIndex = (selectedCard.value?.index ?? 0) + (direction === 'after' ? 1 : -1);
    if (config.get(ZwooConfigKey.SortCards)) {
      return deck.sortedCardAt(nextIndex) !== undefined;
    }
    return deck.cardAt(nextIndex) !== undefined;
  };

  const getNext = (direction: 'before' | 'after'): [Card | undefined, number] => {
    const nextIndex = (selectedCard.value?.index ?? 0) + (direction === 'after' ? 1 : -1);
    if (config.get(ZwooConfigKey.SortCards)) {
      return [deck.sortedCardAt(nextIndex), nextIndex];
    }
    return [deck.cardAt(nextIndex), nextIndex];
  };

  const selectCard = (card: Card, at: number) => {
    if (config.get(ZwooConfigKey.ShowCardsDetail)) {
      selectedCard.value = {
        color: card.color,
        type: card.type,
        index: at
      };
    } else {
      playCard(card);
    }
  };

  const playCard = async (card: Card) => {
    dispatchEvent(ZRPOPCode.PlaceCard, {
      type: card.color,
      symbol: card.type
    });
  };

  const removeCard = async (card: Card) => {
    deck.playCard(card);
    if (config.get(ZwooConfigKey.SortCards)) {
      cards.value = deck.sorted;
      return;
    }
    cards.value = deck.cards;
  };

  const drawCard = () => {
    dispatchEvent(ZRPOPCode.DrawCard, {});
  };

  const reset = () => {
    deck = new CardDeck([]);
    cards.value = [];
    selectedCard.value = undefined;
  };

  deckWatcher.onMessage(_receiveMessage);
  deckWatcher.onReset(reset);
  deckWatcher.onClose(reset);

  return {
    cards,
    selectedCard,
    hasNext,
    getNext,
    selectCard,
    playCard,
    drawCard,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
