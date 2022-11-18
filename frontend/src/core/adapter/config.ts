import { defineStore } from 'pinia';

import { defaultLanguage, setI18nLanguage, supportedLanguages } from '@/i18n';

import { CardThemeIdentifier } from '../services/cards/CardThemeConfig';
import { CardThemeManager } from '../services/cards/ThemeManager';

const languageKey = 'zwoo:lng';
const uiKey = 'zwoo:ui';
const quickMenuKey = 'zwoo:qm';
const sortCardsKey = 'zwoo:sc';
const showCardDetailKey = 'zwoo:cd';
const themeKey = 'zwoo:th';

const changeLanguage = (lng: string) => {
  setI18nLanguage(lng);
  localStorage.setItem(languageKey, lng);
};

const changeUIMode = (isDark: boolean) => {
  localStorage.setItem(uiKey, isDark ? 'dark' : 'light');
  if (isDark) {
    document.querySelector('html')?.classList.add('dark');
  } else {
    document.querySelector('html')?.classList.remove('dark');
  }
};

const changeFullscreen = (enabled: boolean) => {
  if (enabled) {
    document.documentElement.requestFullscreen();
  } else if (document.fullscreenElement) {
    document.exitFullscreen();
  }
};

export const useConfig = defineStore('config', {
  state: () => {
    return {
      useDarkMode: false,
      language: 'en',
      useFullScreen: false,
      showQuickMenu: false,
      sortCards: false,
      showCardDetail: false,
      cardTheme: '__default__',
      cardThemeVariant: '@auto'
    };
  },

  actions: {
    setDarkMode(isEnabled: boolean) {
      this.useDarkMode = isEnabled;
      changeUIMode(isEnabled);
    },
    setLanguage(lng: string) {
      this.language = lng;
      changeLanguage(lng);
    },
    setFullScreen(enabled: boolean) {
      this.useFullScreen = enabled;
      changeFullscreen(enabled);
    },
    setQuickMenu(visible: boolean) {
      this.showQuickMenu = visible;
      localStorage.setItem(quickMenuKey, visible ? 'on' : 'off');
    },
    setSortCards(sort: boolean) {
      this.sortCards = sort;
      localStorage.setItem(sortCardsKey, sort ? 'on' : 'off');
    },
    setShowCardDetail(show: boolean) {
      this.showCardDetail = show;
      localStorage.setItem(showCardDetailKey, show ? 'on' : 'off');
    },
    setTheme(theme: CardThemeIdentifier) {
      this.cardTheme = theme.name;
      this.cardThemeVariant = theme.variant;
      localStorage.setItem(themeKey, JSON.stringify(theme));
    },
    configure() {
      let storedLng = localStorage.getItem(languageKey);
      if (!storedLng) {
        const userLng = navigator.language.split('-')[0]?.toLowerCase();
        storedLng = supportedLanguages.includes(userLng) ? userLng : defaultLanguage;
      }
      setI18nLanguage(storedLng);

      const rawStoredDarkMode = localStorage.getItem(uiKey);
      const storedDarkMode = rawStoredDarkMode
        ? rawStoredDarkMode === 'dark'
        : window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
      changeUIMode(storedDarkMode);

      this.$patch({
        useDarkMode: storedDarkMode,
        language: storedLng
      });
      setTimeout(() => this.asyncSetup());
    },
    async asyncSetup() {
      let storedShowCardDetail = localStorage.getItem(showCardDetailKey);
      if (!storedShowCardDetail) {
        const hoverNotAvailable = 'ontouchstart' in document.documentElement;
        storedShowCardDetail = hoverNotAvailable ? 'on' : 'off';
        this.setShowCardDetail(hoverNotAvailable);
      }

      const storedTheme = localStorage.getItem(themeKey);
      let parsedTheme = {} as CardThemeIdentifier;
      if (storedTheme) {
        parsedTheme = JSON.parse(storedTheme);
      }
      if (!parsedTheme.name || !parsedTheme.variant) {
        parsedTheme = await CardThemeManager.global.getDefaultTheme();
      }

      this.$patch({
        showQuickMenu: localStorage.getItem(quickMenuKey) === 'on',
        sortCards: localStorage.getItem(sortCardsKey) === 'on',
        showCardDetail: storedShowCardDetail === 'on',
        cardTheme: parsedTheme.name ?? null,
        cardThemeVariant: parsedTheme.variant ?? null
      });
    }
  }
});
