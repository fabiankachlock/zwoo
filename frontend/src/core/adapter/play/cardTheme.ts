import { CardTheme } from '@/core/services/cards/CardTheme';
import { CARD_THEME_VARIANT_AUTO, colorModeToVariant } from '@/core/services/cards/CardThemeConfig';
import { CardThemeManager } from '@/core/services/cards/ThemeManager';
import { ref, watch } from 'vue';
import { useConfig } from '../config';
import { useGameConfig } from '../game';

const DEBOUNCE_TIME = 1000;
const ThemeManager = CardThemeManager.global;
let LoadedTheme: CardTheme | undefined;

export const useCardTheme = () => {
  const { useDarkMode } = useConfig();
  const { cardTheme, cardThemeVariant } = useGameConfig();
  const theme = ref<CardTheme>(new CardTheme('', '', {}));
  let debounceTimeout: number | undefined = undefined;

  watch([() => useDarkMode, () => cardTheme, () => cardThemeVariant], ([useDarkMode, cardTheme, themeVariant]) => {
    const newTheme = cardTheme;
    const newVariant = themeVariant === CARD_THEME_VARIANT_AUTO ? colorModeToVariant(useDarkMode) : themeVariant;

    if (LoadedTheme?.name === newTheme && LoadedTheme.variant === newVariant) return; // already loaded;
    // load new Theme

    if (debounceTimeout) {
      clearTimeout(debounceTimeout);
    }
    debounceTimeout = setTimeout(() => loadTheme(newTheme, newVariant), DEBOUNCE_TIME);
  });

  const loadTheme = async (name: string, variant: string) => {
    debounceTimeout = undefined;
    LoadedTheme = await ThemeManager.loadTheme(name, variant);
    theme.value = LoadedTheme;
  };

  loadTheme(cardTheme, cardThemeVariant === CARD_THEME_VARIANT_AUTO ? colorModeToVariant(useDarkMode) : cardThemeVariant);

  return {
    theme
  };
};
