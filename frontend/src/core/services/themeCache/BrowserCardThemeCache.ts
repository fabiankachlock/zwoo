import { QueuedCache } from '@/core/helper/QueuedCache';

import { CardThemeCache } from '../../domain/cards/CardThemeCache';

export class BrowserCardThemeCache<T> implements CardThemeCache<T> {
  private memoryCache: QueuedCache<T>;

  private underlyingCache?: Cache;

  constructor() {
    this.memoryCache = new QueuedCache(3);
  }

  private async getCache(): Promise<Cache> {
    if (this.underlyingCache) return this.underlyingCache;
    this.underlyingCache = await caches.open('zwoo-card-themes');
    return this.underlyingCache;
  }

  private createCacheKey(key: string): Request {
    return new Request(`http://zwoo/__virtual__/themes/${key}`);
  }

  private createCacheData(theme: T): Response {
    return new Response(JSON.stringify(theme));
  }

  private async getThemeFromData(data: Response): Promise<T> {
    return await data.json();
  }

  public async get(key: string): Promise<T | undefined> {
    const result = this.memoryCache.get(key);
    if (result) return result;

    const cache = await this.getCache();
    const cacheResult = await cache.match(this.createCacheKey(key));
    if (cacheResult) return this.getThemeFromData(cacheResult);
    return undefined;
  }

  public async set(key: string, value: T): Promise<T> {
    const response = await this.memoryCache.set(key, value);
    const cache = await this.getCache();
    cache.put(this.createCacheKey(key), this.createCacheData(value));
    return response;
  }

  public async getOrSet(key: string, setter: () => T): Promise<T> {
    const response = await this.get(key);
    if (response) {
      return response;
    }
    return this.set(key, setter());
  }

  public async getOrSetAsync(key: string, setter: () => Promise<T>): Promise<T> {
    const response = await this.get(key);
    if (response) {
      return response;
    }
    const value = await setter();
    return this.set(key, value);
  }

  public async clean(): Promise<void> {
    return this.memoryCache.clean();
  }
}
