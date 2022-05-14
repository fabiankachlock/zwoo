import { Card, CardColor } from '@/core/services/game/card';
export const Random = {
  numberInRange(lowerBound: number, higherBound: number) {
    return Math.random() * (higherBound - lowerBound) + lowerBound;
  },

  intInRange(lowerBound: number, higherBound: number) {
    return Math.floor(Random.numberInRange(lowerBound, higherBound));
  },

  card(): Card {
    const color = Random.intInRange(1, 5);
    return {
      color,
      type: color === CardColor.black ? Random.intInRange(14, 15) : Random.intInRange(1, 13)
    };
  }
};
