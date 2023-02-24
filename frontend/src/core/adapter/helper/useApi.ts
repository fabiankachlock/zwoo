import { useRootApp } from '../app';

export const useApi = () => {
  const app = useRootApp();
  return () => app.api;
};
