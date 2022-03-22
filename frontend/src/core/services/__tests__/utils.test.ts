import { arrayDiff, joinQuery } from '../utils';

describe('Utils', () => {
  describe('joinQuery', () => {
    it('should join a query', () => {
      expect(joinQuery({ a: 'a', b: '2' })).toEqual('a=a&b=2');
    });
  });

  describe('arrayDiff', () => {
    it('should find new items', () => {
      expect(arrayDiff([1, 2, 3], [1, 2, 3, 4]).added).toEqual([4]);
      expect(arrayDiff([], [2, 3, 4]).added).toEqual([2, 3, 4]);
      expect(arrayDiff([1, 2], [1, 2]).added).toEqual([]);
    });

    it('should find remove items', () => {
      expect(arrayDiff([1, 2, 3, 4], [1, 2, 3]).removed).toEqual([4]);
      expect(arrayDiff([2, 3, 4], []).removed).toEqual([2, 3, 4]);
      expect(arrayDiff([1, 2], [1, 2]).removed).toEqual([]);
    });

    it('should find unmodified items', () => {
      expect(arrayDiff([1, 2, 3], [1, 2, 3, 4]).unmodified).toEqual([1, 2, 3]);
      expect(arrayDiff([], [2, 3, 4]).unmodified).toEqual([]);
      expect(arrayDiff([1, 2], [1, 2]).unmodified).toEqual([1, 2]);
    });

    it('should do all together', () => {
      expect(arrayDiff([0, 2, 3, 4], [1, 2, 3, 5])).toEqual({
        added: [1, 5],
        removed: [0, 4],
        unmodified: [2, 3]
      });
    });

    it('should use custom matchers', () => {
      expect(arrayDiff([-1, 2], [1, 2, -3], (a, b) => a === -b)).toEqual({
        added: [2, -3],
        removed: [2],
        unmodified: [-1] // old 1 gets replaced by new (equal) -1
      });
    });
  });
});
