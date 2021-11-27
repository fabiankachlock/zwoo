import { defineStore } from 'pinia';
import { CardDeck } from '../../services/game/deck';
import { Card } from '../../type/game';
import { useConfig } from '../config';

export const useGameCardDeck = defineStore('game-cards', {
  state: () => ({
    cards: [] as Card[],
    selectedCard: undefined as Card | undefined,
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
      const config = useConfig();
      if (config.sortCards) {
        return this._deck.nextSorted(this.selectedCard?.id ?? '', after);
      }
      return this._deck.next(this.selectedCard?.id ?? '', after);
    },
    selectCard(id: string) {
      this.selectedCard = this._deck.cards.find(card => card.id === id);
    },
    playCard(id: string) {
      const playedCard = this._deck.cards.find(card => card.id === id);

      if (playedCard) {
        // TODO: Call Api
        const config = useConfig();
        this._deck.playCard(id);
        if (config.sortCards) {
          this.cards = this._deck.sorted;
          return;
        }
        this.cards = this._deck.cards;
      }
    }
  }
});

