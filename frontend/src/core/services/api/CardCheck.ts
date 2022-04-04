import { Card } from '@/core/type/game';
import Logger from '../logging/logImport';

export class CardChecker {
  // eslint-disable-next-line
  static async canPlayCard(card: Card): Promise<boolean> {
    Logger.Api.debug('mocking play card response');
    return Math.random() > 0.5;
  }
}
