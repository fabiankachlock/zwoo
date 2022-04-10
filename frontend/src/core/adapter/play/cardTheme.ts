import { useColorTheme } from '@/composables/colorTheme';
import { CardTheme } from '@/core/services/cards/CardTheme';
import { CARD_THEME_VARIANT_AUTO } from '@/core/services/cards/CardThemeConfig';
import { CardThemeManager } from '@/core/services/cards/ThemeManager';
import { CreateUseHook } from '@/core/services/helper/CreateUseHook';
import Logger from '@/core/services/logging/logImport';
import { ref, watch } from 'vue';
import { useGameConfig } from '../game';

const DEBOUNCE_TIME = 1000;
const ThemeManager = CardThemeManager.global;
let LoadedTheme: CardTheme | undefined;

export const useCardTheme = CreateUseHook(() => {
  const colorMode = useColorTheme();
  const { cardTheme, cardThemeVariant } = useGameConfig();
  const theme = ref<CardTheme>(new CardTheme('', '', {}));
  let debounceTimeout: number | undefined = undefined;

  watch([colorMode, () => cardTheme, () => cardThemeVariant], ([colorMode, cardTheme, themeVariant]) => {
    Logger.Theme.debug('changed user settings');
    const newTheme = cardTheme;
    const newVariant = themeVariant === CARD_THEME_VARIANT_AUTO ? colorMode : themeVariant;

    if (LoadedTheme?.name === newTheme && LoadedTheme.variant === newVariant) {
      // already loaded
      Logger.Theme.debug('nothing changed');
      return;
    }

    // load new Theme
    if (debounceTimeout) {
      Logger.Theme.debug('cancelling scheduled load');
      clearTimeout(debounceTimeout);
    }
    debounceTimeout = setTimeout(() => loadTheme(newTheme, newVariant), DEBOUNCE_TIME);
    Logger.Theme.debug(`scheduled load (timeout: ${debounceTimeout})`);
  });

  const loadTheme = async (name: string, variant: string) => {
    debounceTimeout = undefined;
    Logger.Theme.debug('loading new theme');
    LoadedTheme = await ThemeManager.loadTheme(name, variant);
    theme.value = LoadedTheme;
  };

  Logger.Theme.debug('initial setup theme load');
  loadTheme(cardTheme, cardThemeVariant === CARD_THEME_VARIANT_AUTO ? colorMode.value : cardThemeVariant);

  return {
    theme
  };
});
