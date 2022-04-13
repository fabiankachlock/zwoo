import { useConfig } from '@/core/adapter/config';
import { computed } from 'vue';

export const useColorTheme = () => {
  const config = useConfig();
  return computed(() => (config.useDarkMode ? 'dark' : 'light'));
};
