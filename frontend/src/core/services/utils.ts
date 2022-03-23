import { LocationQuery } from 'vue-router';

export const joinQuery = (q: LocationQuery): string =>
  Object.entries(q)
    .map(([k, v]) => `${k}=${v}`)
    .join('&');

type DiffResult<T> = {
  added: T[];
  removed: T[];
  unmodified: T[];
};

export const arrayDiff = <T>(before: T[], after: T[], equals?: (a: T, b: T) => boolean): DiffResult<T> => {
  const added = [] as T[];
  const removed = [] as T[];
  const unmodified = [] as T[];

  const areEqual = equals ? equals : (a: T, b: T) => a === b;
  const usedAfterIndexes = {} as Record<string, boolean>;

  for (const item of before) {
    let isUsed = false;
    for (const [index, newItem] of after.entries()) {
      if (areEqual(item, newItem)) {
        unmodified.push(item);
        usedAfterIndexes[index] = true;
        isUsed = true;
        break;
      }
    }
    if (!isUsed) {
      removed.push(item);
    }
  }

  for (const [index, newItem] of after.entries()) {
    if (!usedAfterIndexes[index]) {
      added.push(newItem);
    }
  }

  return {
    added,
    unmodified,
    removed
  };
};
