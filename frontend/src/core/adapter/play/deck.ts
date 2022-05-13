import { Card } from '@/core/services/game/card';
import { defineStore } from 'pinia';
import { CardDeck } from '../../services/game/deck';
import { useConfig } from '../config';

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
    prefetchNext(after: boolean): Card | undefined {
      const nextIndex = (this.selectedCard?.index ?? 0) + (after ? 1 : -1);
      const config = useConfig();
      if (config.sortCards) {
        return this._deck.sortedCardAt(nextIndex);
      }
      return this._deck.cardAt(nextIndex);
    },
    selectCard(card: Card, at: number) {
      this.selectedCard = {
        ...card,
        index: at
      };
    },
    playCard(card: Card) {
      // TODO: Call Api
      const config = useConfig();
      this._deck.playCard(card);
      if (config.sortCards) {
        this.cards = this._deck.sorted;
        return;
      }
      this.cards = this._deck.cards;
    }
  }
});
