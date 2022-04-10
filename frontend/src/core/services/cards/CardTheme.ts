import { Card } from '../game/card';
import { CardThemeConfig } from './CardThemeConfig';

export class CardTheme {
  constructor(public readonly name: string, public readonly variant: string, private readonly data: CardThemeConfig) {}

  public getCard(card: Card): string {
    // return `${this.name} ${this.variant} ${this.cardToURI(card)}`;
    return this.data[this.cardToURI(card)] ?? '';
  }

  private cardToURI(card: Card): string {
    return `front_${card.color}_${card.type.toString(16)}`;
  }
}
