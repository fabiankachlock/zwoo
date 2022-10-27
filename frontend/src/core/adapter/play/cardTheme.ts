import { reactive, ref, watch } from 'vue';

import { useColorTheme } from '@/core/adapter/helper/useColorTheme';
import { CardTheme } from '@/core/services/cards/CardTheme';
import { CARD_THEME_VARIANT_AUTO, CardThemeInformation } from '@/core/services/cards/CardThemeConfig';
import { CardThemeManager } from '@/core/services/cards/ThemeManager';
import { CreateUseHook } from '@/core/services/helper/CreateUseHook';
import Logger from '@/core/services/logging/logImport';

import { useConfig } from '../config';

const DEBOUNCE_TIME = 1000;
const ThemeManager = CardThemeManager.global;

export const useCardTheme = CreateUseHook(() => {
  const colorMode = useColorTheme();
  const config = useConfig();
  const theme = ref<CardTheme>(new CardTheme('', '', {}, {} as CardThemeInformation));
  let debounceTimeout: number | undefined = undefined;

  watch([colorMode, () => config.cardTheme, () => config.cardThemeVariant], ([colorMode, cardTheme, themeVariant]) => {
    Logger.Theme.debug('changed user settings');
    const newTheme = cardTheme;
    const newVariant = themeVariant === CARD_THEME_VARIANT_AUTO ? colorMode : themeVariant;

    // load new Theme
    if (debounceTimeout) {
      Logger.Theme.debug('cancelling theme change, waiting for debounce timeout');
      return;
    }
    loadTheme(newTheme, newVariant);
    debounceTimeout = setTimeout(() => {
      debounceTimeout = undefined;
      applyTheme();
    }, DEBOUNCE_TIME);
    Logger.Theme.debug(`scheduled load (timeout: ${debounceTimeout})`);
  });

  const applyTheme = () => {
    Logger.Theme.debug('applying theme changes ');
    const newVariant = config.cardThemeVariant === CARD_THEME_VARIANT_AUTO ? colorMode.value : config.cardThemeVariant;
    if (theme.value.variant !== newVariant || theme.value.name !== config.cardTheme) {
      loadTheme(config.cardTheme, newVariant);
    } else {
      Logger.Theme.debug('no changes to apply');
    }
  };

  const loadTheme = async (name: string, variant: string) => {
    debounceTimeout = undefined;
    Logger.Theme.debug('loading new theme');
    const loadedTheme = await ThemeManager.loadTheme({ name, variant });
    theme.value = loadedTheme;
  };

  const setTheme = (name: string, variant: string) => {
    const newVariant = variant === CARD_THEME_VARIANT_AUTO ? colorMode.value : variant;
    config.setTheme({
      name,
      variant
    });
    loadTheme(name, newVariant);
  };

  Logger.Theme.debug('initial setup theme load');
  loadTheme(config.cardTheme, config.cardThemeVariant === CARD_THEME_VARIANT_AUTO ? colorMode.value : config.cardThemeVariant);

  return reactive({
    theme,
    setTheme,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  });
});
