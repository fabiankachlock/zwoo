import { computed } from 'vue';

import { useRootApp } from '@/core/adapter/app';

export const useIsOnline = () => {
  const app = useRootApp();
  return computed(() => app.environment === 'online');
};

export const useIsOffline = () => {
  const app = useRootApp();
  return computed(() => app.environment === 'offline');
};
