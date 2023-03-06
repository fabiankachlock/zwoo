type CacheItem<T> = {
  lastChanged: number;
  value: T;
};

export class QueuedCache<T> {
  private cache: Record<string, CacheItem<T>> = {};

  constructor(public readonly maxItemCount: number) {}

  public get(key: string): T | undefined {
    return this.cache[key]?.value;
  }

  public set(key: string, value: T): T {
    this.cache[key] = {
      lastChanged: Date.now(),
      value
    };
    this.clean();
    return value;
  }

  public getOrSet(key: string, setter: () => T): T {
    const cachedValue = this.cache[key];
    if (cachedValue) return cachedValue.value;
    return this.set(key, setter());
  }

  public async getOrSetAsync(key: string, setter: () => Promise<T>): Promise<T> {
    const cachedValue = this.cache[key];
    if (cachedValue) return cachedValue.value;
    const newValue = await setter();
    return this.set(key, newValue);
  }

  public clean() {
    const items = Object.keys(this.cache);
    if (items.length < this.maxItemCount) return;
    const sorted = Object.entries(this.cache)
      .map(([key, item]) => ({ changed: item.lastChanged, key }))
      .sort((a, b) => a.changed - b.changed); // sort oldest to newest

    while (sorted.length > this.maxItemCount) {
      delete this.cache[sorted[0].key];
      sorted.shift();
    }
  }
}
