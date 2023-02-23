import { QueuedCache } from '../QueuedCache';

let timer = 0;
Date.now = jest.fn(() => ++timer);

describe('QueuedCache', () => {
  let cache: QueuedCache<string>;

  beforeEach(() => {
    cache = new QueuedCache(3);
  });

  it('should get & set items', () => {
    const value = 'abc';
    cache.set('a', value);
    expect(cache.get('a')).toEqual(value);
  });

  it('should use optional setter', () => {
    const value = 'abc';
    expect(cache.getOrSet('a', () => value)).toEqual(value);
  });

  it('should not use optional setter when already set', () => {
    const value = 'abc';
    cache.set('a', value);
    expect(cache.getOrSet('a', () => 'xxx')).toEqual(value);
  });

  it('should clean automatically', () => {
    cache.set('a', '1');
    cache.set('b', '2');
    cache.set('c', '3');
    cache.set('d', '4');
    expect(cache.get('a')).toBeUndefined();
  });

  it('should respect last changed', () => {
    cache.set('a', '1');
    cache.set('b', '2');
    cache.set('c', '3');
    cache.set('d', '4');
    cache.set('a', '5');
    expect(cache.get('b')).toBeUndefined();
    expect(cache.get('a')).toEqual('5');
  });
});
