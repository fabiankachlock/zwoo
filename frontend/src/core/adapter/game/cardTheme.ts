import { reactive, ref, watch } from 'vue';

import { ZwooConfigKey } from '@/core/adapter/config';
import { useColorTheme } from '@/core/adapter/helper/useColorTheme';
import { CardTheme } from '@/core/domain/cards/CardTheme';
import { CARD_THEME_VARIANT_AUTO, CardThemeInformation } from '@/core/domain/cards/CardThemeConfig';
import { CardThemeManager } from '@/core/domain/cards/ThemeManager';
import { CreateUseHook } from '@/core/helper/CreateUseHook';
import Logger from '@/core/services/logging/logImport';

import { useConfig } from '../config';

const DEBOUNCE_TIME = 1000;
const ThemeManager = CardThemeManager.global;

export const useCardTheme = CreateUseHook(() => {
  const colorMode = useColorTheme();
  const config = useConfig();
  // TODO: mark theme as raw
  const theme = ref<CardTheme>(new CardTheme('', '', {}, {} as CardThemeInformation));
  let debounceTimeout: number | undefined = undefined;

  watch([colorMode, () => config.get(ZwooConfigKey.CardsTheme)], ([colorMode, cardTheme]) => {
    Logger.Theme.debug('changed user settings');
    const newTheme = cardTheme?.name;
    const newVariant = cardTheme?.variant === CARD_THEME_VARIANT_AUTO ? colorMode : cardTheme?.variant;

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
    const currentTheme = config.get(ZwooConfigKey.CardsTheme);
    const newVariant = currentTheme.variant === CARD_THEME_VARIANT_AUTO ? colorMode.value : currentTheme.variant;
    if (theme.value.variant !== newVariant || theme.value.name !== currentTheme.name) {
      loadTheme(currentTheme.name, newVariant);
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
    config.set(ZwooConfigKey.CardsTheme, {
      name,
      variant
    });
    loadTheme(name, newVariant);
  };

  Logger.Theme.debug('initial setup theme load');
  const currentTheme = config.get(ZwooConfigKey.CardsTheme);
  loadTheme(currentTheme.name, currentTheme.variant === CARD_THEME_VARIANT_AUTO ? colorMode.value : currentTheme.variant);

  return reactive({
    theme,
    setTheme,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  });
});
