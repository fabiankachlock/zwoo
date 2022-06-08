export const CacheTTL = {
  millisecond: 1,
  second: 1000,
  minute: 1000 * 60,
  hour: 1000 * 60 * 60
};

export const useDebounce = <T>(creator: () => T | Promise<T>, ttl: number): (() => Promise<T>) => {
  let lastCall = -Infinity;
  let value: T;

  const execute = async () => {
    value = await creator();
    lastCall = performance.now();
  };

  return async () => {
    if (lastCall + ttl < performance.now()) {
      await execute();
    }
    return value;
  };
};
