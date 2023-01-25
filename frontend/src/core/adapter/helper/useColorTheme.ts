import { computed } from 'vue';

import { useConfig, ZwooConfigKey } from '@/core/adapter/config';

export const useColorTheme = () => {
  const config = useConfig();
  return computed(() => config.get(ZwooConfigKey.UiMode));
};
