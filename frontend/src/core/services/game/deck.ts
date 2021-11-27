import { Card } from '@/core/type/game';

export class CardDeck {
  private content: Card[];

  get cards(): Card[] {
    return [...this.content];
  }

  get sorted(): Card[] {
    // TODO: Sort cards
    return [...this.content];
  }

  constructor(cards: Card[]) {
    this.content = cards;
  }

  playCard = (id: string): Card | undefined => {
    const index = this.content.findIndex(card => card.id === id);
    if (index >= 0) {
      const card = this.content[index];
      this.content.splice(index, 1);
      return card;
    }
    return undefined;
  };

  pushCard = (card: Card) => {
    const existingIndex = this.content.findIndex(_card => _card.id === card.id);
    if (existingIndex < 0) {
      this.content.push(card);
    }
  };

  next = (id: string, after = true): Card | undefined => {
    return this.findNextCard(this.cards, id, after ? 1 : -1);
  };

  nextSorted = (id: string, after = true): Card | undefined => {
    return this.findNextCard(this.sorted, id, after ? 1 : -1);
  };

  private findNextCard = (cards: Card[], id: string, indexChange: number): Card | undefined => {
    const index = cards.findIndex(card => card.id === id);
    if (index < 0) {
      return undefined;
    }

    const newIndex = index + indexChange;
    if (newIndex > 0 && newIndex < cards.length - 1) {
      return cards[newIndex];
    }
    return undefined;
  };
}
