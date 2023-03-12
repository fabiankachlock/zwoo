export interface CardThemeCache<T> {
  get(key: string): Promise<T | undefined>;
  set(key: string, value: T): Promise<T>;
  getOrSet(key: string, setter: () => T): Promise<T>;
  getOrSetAsync(key: string, setter: () => Promise<T>): Promise<T>;
  clean(): Promise<void>;
}
