/**
 *	This is an LRU Cache implementation that is used to store the search results
 *
 * This implementation is the same used in vitepresses original VPLocalSearchBox.vue
 * https://github.com/vuejs/vitepress/blob/main/src/client/theme-default/support/lru.ts
 * MIT License | Copyright (c) 2019-present, Yuxi (Evan) You
 */

// adapted from https://stackoverflow.com/a/46432113/11613622

export class LRUCache<K, V> {
  private max: number;
  private cache: Map<K, V>;

  constructor(max: number = 10) {
    this.max = max;
    this.cache = new Map<K, V>();
  }

  get(key: K): V | undefined {
    const item = this.cache.get(key);
    if (item !== undefined) {
      // refresh key
      this.cache.delete(key);
      this.cache.set(key, item);
    }
    return item;
  }

  set(key: K, val: V): void {
    // refresh key
    if (this.cache.has(key)) this.cache.delete(key);
    // evict oldest
    else if (this.cache.size === this.max) this.cache.delete(this.first()!);
    this.cache.set(key, val);
  }

  first(): K | undefined {
    return this.cache.keys().next().value;
  }

  clear(): void {
    this.cache.clear();
  }
}
