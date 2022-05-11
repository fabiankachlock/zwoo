import { Card } from '../../game/card';
import { NodeBasedQueue } from '../../helper/NodeBasedQueue';

export class CardSorter {
  private cardToNumber = (card: Card): number => card.color * 100 + card.type;
  private numberToCard = (id: number): Card => ({
    id: '',
    color: Math.floor(id / 100),
    type: id / 100 - Math.floor(id / 100)
  });

  private static RadixSortBase = 10;

  private createRadixSortBuckets(): NodeBasedQueue<number>[] {
    return [...new Array(CardSorter.RadixSortBase).fill(0).map(() => new NodeBasedQueue<number>())];
  }

  radixSort(numbers: number[], max?: number): number[] {
    const mainQueue = NodeBasedQueue.fromArray(numbers);
    const buckets = this.createRadixSortBuckets();
    const iterations = Math.floor(Math.log10(max ?? Math.max(...numbers)));
    for (let pointer = 0; pointer < iterations; pointer++) {
      const exp = Math.pow(10, pointer);
      while (!mainQueue.isEmpty) {
        // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
        const num = mainQueue.dequeue()!;
        buckets[~~((num / exp) % 10)].enqueue(num);
      }
      for (let idx = 0; idx < CardSorter.RadixSortBase; idx++) {
        while (!buckets[idx].isEmpty) {
          // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
          mainQueue.enqueue(buckets[idx].dequeue()!);
        }
      }
    }
    return mainQueue.toArray();
  }

  sort(cards: Card[]): Card[] {
    const mappedCards = cards.map(this.cardToNumber);
    const sorted = this.radixSort(mappedCards);
    return sorted.map(this.numberToCard);
  }
}
