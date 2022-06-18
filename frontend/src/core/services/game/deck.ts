import { Card, CardType } from './card';
import { CardSorter } from '../cards/sorting/CardSorter';

export class CardDeck {
  private content: Card[];
  private sortedContent: Card[];

  private sorter = new CardSorter();

  get cards(): Card[] {
    return [...this.content];
  }

  get sorted(): Card[] {
    return [...this.sortedContent];
  }

  constructor(cards: Card[]) {
    this.content = cards;
    this.sortedContent = this.sorter.sort(this.content);
  }

  private cardMatcher = (base: Card) => (card: Card) => {
    if ((base.type === CardType.wild && card.type === CardType.wild) || (base.type === CardType.wild_four && card.type === CardType.wild_four)) {
      return true;
    }
    return base.color === card.color || base.type === card.type;
  };

  hasCard = (card: Card): boolean => {
    return this.content.findIndex(this.cardMatcher(card)) !== undefined;
  };

  playCard = (card: Card): Card | undefined => {
    const index = this.content.findIndex(this.cardMatcher(card));
    if (index >= 0) {
      const card = this.content[index];
      this.content.splice(index, 1);
      this.sortedContent = this.sorter.sort(this.content);
      return card;
    }
    return undefined;
  };

  pushCard = (card: Card): void => {
    this.content.unshift(card);
    this.sortedContent = this.sorter.sort(this.content);
  };

  cardAt = (index: number): Card | undefined => {
    return this.content[index];
  };

  sortedCardAt = (index: number): Card | undefined => {
    return this.sorted[index];
  };
}
