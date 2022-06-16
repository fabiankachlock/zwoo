import { Card, CardColor } from '@/core/services/game/card';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { CardDeck } from '../../services/game/deck';
import { useConfig } from '../config';
import { InGameModal } from './modal';
import { useModalResponse } from './util/awaitModalResponse';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

const deckWatcher = new MonolithicEventWatcher(ZRPOPCode.GameStarted, ZRPOPCode.GetCard, ZRPOPCode.RemoveCard);

export const useGameCardDeck = defineStore('game-cards', () => {
  const cards = ref<Card[]>([]);
  const selectedCard = ref<(Card & { index: number }) | undefined>(undefined);
  let deck = new CardDeck([]);
  const config = useConfig();

  const setState = (newCards: Card[]) => {
    const config = useConfig();
    deck = new CardDeck(newCards);
    if (config.sortCards) {
      cards.value = deck.sorted;
      return;
    }
    cards.value = deck.cards;
  };

  const addCard = (card: Card) => {
    const config = useConfig();
    deck.pushCard(card);
    if (config.sortCards) {
      cards.value = deck.sorted;
      return;
    }
    cards.value = deck.cards;
  };

  const hasNext = (direction: 'before' | 'after'): boolean => {
    const nextIndex = (selectedCard.value?.index ?? 0) + (direction === 'after' ? 1 : -1);
    if (config.sortCards) {
      return deck.sortedCardAt(nextIndex) !== undefined;
    }
    return deck.cardAt(nextIndex) !== undefined;
  };

  const getNext = (direction: 'before' | 'after'): [Card | undefined, number] => {
    const nextIndex = (selectedCard.value?.index ?? 0) + (direction === 'after' ? 1 : -1);
    if (config.sortCards) {
      return [deck.sortedCardAt(nextIndex), nextIndex];
    }
    return [deck.cardAt(nextIndex), nextIndex];
  };

  const selectCard = (card: Card, at: number) => {
    if (config.showCardDetail) {
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
    deck.playCard(card);
    if (card.color === CardColor.black) {
      const selectedColor = await useModalResponse(InGameModal.ColorPicker);
      if (!selectedColor) return;
      card.color = selectedColor;
    }
    if (config.sortCards) {
      cards.value = deck.sorted;
      return;
    }
    cards.value = deck.cards;
  };

  return {
    hasNext,
    getNext,
    selectCard,
    playCard,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
