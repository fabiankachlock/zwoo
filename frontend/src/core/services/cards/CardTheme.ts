import { Card } from '../game/card';
import { CardDescriptor, CardThemeData } from './CardThemeConfig';

export class CardTheme {
  constructor(public readonly name: string, public readonly variant: string, private readonly data: CardThemeData) {}

  public getCard(card: Card | CardDescriptor): string {
    if (typeof card === 'string') {
      return this.data[card];
    }
    return this.data[this.cardToURI(card)] ?? '';
  }

  private cardToURI(card: Card): string {
    return `front_${card.color}_${card.type.toString(16)}`;
  }
}
