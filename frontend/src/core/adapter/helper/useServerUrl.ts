import { useRootApp } from '@/core/adapter/app';

export const useServerUrl = (id: string) => {
  const app = useRootApp();
  let target: Pick<URL, 'protocol' | 'host'> = window.location;
  if (app.environment === 'local') {
    target = new URL(app.api.getServer());
  }
  return `${target.protocol}//${target.host}/join/${id}`;
};
