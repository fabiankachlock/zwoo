import { computed } from 'vue';

import { useConfig } from '@/core/adapter/config';

export const useColorTheme = () => {
  const config = useConfig();
  return computed(() => (config.useDarkMode ? 'dark' : 'light'));
};
