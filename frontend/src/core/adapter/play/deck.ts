import { Card, CardColor } from '@/core/services/game/card';
import { defineStore } from 'pinia';
import { CardDeck } from '../../services/game/deck';
import { useConfig } from '../config';
import { useGameState } from './gameState';
import { InGameModal } from './modal';
import { useModalResponse } from './util/awaitModalResponse';

export const useGameCardDeck = defineStore('game-cards', {
  state: () => ({
    cards: [] as Card[],
    selectedCard: undefined as (Card & { index: number }) | undefined,
    _deck: new CardDeck([])
  }),
  actions: {
    setState(cards: Card[]) {
      const config = useConfig();
      this._deck = new CardDeck(cards);
      if (config.sortCards) {
        this.cards = this._deck.sorted;
        return;
      }
      this.cards = this._deck.cards;
    },
    addCard(card: Card) {
      const config = useConfig();
      this._deck.pushCard(card);
      if (config.sortCards) {
        this.cards = this._deck.sorted;
        return;
      }
      this.cards = this._deck.cards;
    },
    hasNext(direction: 'before' | 'after'): boolean {
      const nextIndex = (this.selectedCard?.index ?? 0) + (direction === 'after' ? 1 : -1);
      const config = useConfig();
      if (config.sortCards) {
        return this._deck.sortedCardAt(nextIndex) !== undefined;
      }
      return this._deck.cardAt(nextIndex) !== undefined;
    },
    getNext(direction: 'before' | 'after'): [Card | undefined, number] {
      const nextIndex = (this.selectedCard?.index ?? 0) + (direction === 'after' ? 1 : -1);
      const config = useConfig();
      if (config.sortCards) {
        return [this._deck.sortedCardAt(nextIndex), nextIndex];
      }
      return [this._deck.cardAt(nextIndex), nextIndex];
    },
    selectCard(card: Card, at: number) {
      const config = useConfig();
      if (config.showCardDetail) {
        this.selectedCard = {
          color: card.color,
          type: card.type,
          index: at
        };
      } else {
        this.playCard(card);
      }
    },
    async playCard(card: Card) {
      this._deck.playCard(card);
      if (card.color === CardColor.black) {
        const selectedColor = await useModalResponse(InGameModal.ColorPicker);
        if (!selectedColor) return;
        card.color = selectedColor;
      }
      const config = useConfig();
      // TODO: just temp - remove later
      const gameState = useGameState();
      gameState.mainCard = card;
      if (config.sortCards) {
        this.cards = this._deck.sorted;
        return;
      }
      this.cards = this._deck.cards;
    },
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  }
});
