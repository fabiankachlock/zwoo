import { QueuedCache } from '@/core/helper/QueuedCache';

import { CardThemeCache } from '../../domain/cards/CardThemeCache';

class MockCache implements Cache {
  private _store = new Map();

  public add(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  public addAll(): Promise<void> {
    throw new Error('Method not implemented.');
  }

  public delete(request: RequestInfo | URL): Promise<boolean> {
    return Promise.resolve(this._store.delete(request));
  }

  public keys(): Promise<readonly Request[]> {
    throw new Error('Method not implemented.');
  }

  public match(request: RequestInfo | URL): Promise<Response | undefined> {
    return Promise.resolve(this._store.get(request));
  }

  public matchAll(): Promise<readonly Response[]> {
    throw new Error('Method not implemented.');
  }

  public put(request: RequestInfo | URL, response: Response): Promise<void> {
    this._store.set(request, response);
    return Promise.resolve();
  }
}

export class BrowserCardThemeCache<T> implements CardThemeCache<T> {
  private memoryCache: QueuedCache<T>;
  private underlyingCache?: Cache;

  constructor() {
    this.memoryCache = new QueuedCache(3);
  }

  private async getCache(): Promise<Cache> {
    if (this.underlyingCache) return this.underlyingCache;
    try {
      this.underlyingCache = await caches.open('zwoo-card-themes');
    } catch {
      console.warn('using fallback cache');
      this.underlyingCache = new MockCache();
    }
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
