import { LocationQuery } from 'vue-router';

export const joinQuery = (q: LocationQuery): string =>
  Object.entries(q)
    .map(([k, v]) => `${k}=${v}`)
    .join('&');

