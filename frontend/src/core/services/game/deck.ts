import { Card } from './card';
import { CardSorter } from '../cards/sorting/CardSorter';

export class CardDeck {
  private content: Card[];

  private sorter = new CardSorter();

  get cards(): Card[] {
    return [...this.content];
  }

  get sorted(): Card[] {
    // TODO: fix sorting
    return [...this.content];
    return this.sorter.sort(this.content);
  }

  constructor(cards: Card[]) {
    this.content = cards;
  }

  // TODO: colorChangeCards!!!
  private cardMatcher = (base: Card) => (card: Card) => base.color === card.color || base.type === card.type;

  hasCard = (card: Card): boolean => {
    return this.content.findIndex(this.cardMatcher(card)) !== undefined;
  };

  playCard = (card: Card): Card | undefined => {
    const index = this.content.findIndex(this.cardMatcher(card));
    if (index >= 0) {
      const card = this.content[index];
      this.content.splice(index, 1);
      return card;
    }
    return undefined;
  };

  pushCard = (card: Card): void => {
    this.content.unshift(card);
  };

  cardAt = (index: number): Card | undefined => {
    return this.content[index];
  };

  sortedCardAt = (index: number): Card | undefined => {
    return this.sorted[index];
  };
}
