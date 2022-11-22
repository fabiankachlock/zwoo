// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-nocheck
import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { CardThemeIdentifier } from '@/core/services/cards/CardThemeConfig';
import { defaultLanguage, supportedLanguages } from '@/i18n';

const languageKey = 'zwoo:lng';
const uiKey = 'zwoo:ui';
const quickMenuKey = 'zwoo:qm';
const sortCardsKey = 'zwoo:sc';
const showCardDetailKey = 'zwoo:cd';
const themeKey = 'zwoo:th';

export default {
  up() {
    const config = useConfig();
    let storedLng = localStorage.getItem(languageKey);
    if (!storedLng) {
      const userLng = navigator.language.split('-')[0]?.toLowerCase();
      storedLng = supportedLanguages.includes(userLng) ? userLng : defaultLanguage;
    }
    config.set(ZwooConfigKey.Language, storedLng);

    const rawStoredDarkMode = localStorage.getItem(uiKey);
    const storedDarkMode = rawStoredDarkMode
      ? rawStoredDarkMode === 'dark'
      : window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    config.set(ZwooConfigKey.UiMode, storedDarkMode ? 'dark' : 'light');

    let storedShowCardDetail = localStorage.getItem(showCardDetailKey);
    if (!storedShowCardDetail) {
      const hoverNotAvailable = 'ontouchstart' in document.documentElement;
      storedShowCardDetail = hoverNotAvailable ? 'on' : 'off';
    }
    config.set(ZwooConfigKey.ShowCardsDetail, storedShowCardDetail === 'on');

    const storedTheme = localStorage.getItem(themeKey);
    let parsedTheme = {} as CardThemeIdentifier;
    if (storedTheme) {
      parsedTheme = JSON.parse(storedTheme);
    }
    if (parsedTheme.name && parsedTheme.variant) {
      config.set(ZwooConfigKey.CardsTheme, parsedTheme);
    }

    config.set(ZwooConfigKey.QuickMenu, localStorage.getItem(quickMenuKey) === 'on');
    config.set(ZwooConfigKey.SortCards, localStorage.getItem(sortCardsKey) === 'on');
    config.set(ZwooConfigKey.UserDefaults, localStorage.get('zwoo:ud') ?? '');
    config.set(ZwooConfigKey.DevSettings, localStorage.get('zwoo:dev-settings') === 'true');
    config.set(ZwooConfigKey.Logging, localStorage.get('zwoo:logging') ?? '');
    return;
  },
  async down() {
    const config = useConfig();
    let storedLng = localStorage.getItem(languageKey);
    if (!storedLng) {
      const userLng = navigator.language.split('-')[0]?.toLowerCase();
      storedLng = supportedLanguages.includes(userLng) ? userLng : defaultLanguage;
    }
    config.setLanguage(storedLng);

    const rawStoredDarkMode = localStorage.getItem(uiKey);
    const storedDarkMode = rawStoredDarkMode
      ? rawStoredDarkMode === 'dark'
      : window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    config.setDarkMode(storedDarkMode);

    config.$patch({
      useDarkMode: storedDarkMode,
      language: storedLng
    });

    let storedShowCardDetail = localStorage.getItem(showCardDetailKey);
    if (!storedShowCardDetail) {
      const hoverNotAvailable = 'ontouchstart' in document.documentElement;
      storedShowCardDetail = hoverNotAvailable ? 'on' : 'off';
      config.setShowCardDetail(hoverNotAvailable);
    }

    const storedTheme = localStorage.getItem(themeKey);
    let parsedTheme = {} as CardThemeIdentifier;
    if (storedTheme) {
      parsedTheme = JSON.parse(storedTheme);
    }
    if (!parsedTheme.name || !parsedTheme.variant) {
      parsedTheme = await CardThemeManager.global.getDefaultTheme();
    }

    config.$patch({
      showQuickMenu: localStorage.getItem(quickMenuKey) === 'on',
      sortCards: localStorage.getItem(sortCardsKey) === 'on',
      showCardDetail: storedShowCardDetail === 'on',
      cardTheme: parsedTheme.name ?? null,
      cardThemeVariant: parsedTheme.variant ?? null
    });
    return;
  }
};
