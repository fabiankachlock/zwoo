import { Card } from '@/core/type/game';

export class CardChecker {
  // eslint-disable-next-line
  static async canPlayCard(card: Card): Promise<boolean> {
    return Math.random() > 0.5;
  }
}
