import { defaultLanguage, setI18nLanguage } from '@/i18n';
import { defineStore } from 'pinia';

const languageKey = 'zwoo:lng';
const uiKey = 'zwoo:ui';
const quickMenuKey = 'zwoo:qm';

const versionInfo = {
  version: process.env.VUE_APP_VERSION as string,
  hash: process.env.VUE_APP_VERSION_HASH as string
};

const changeLanguage = (lng: string) => {
  setI18nLanguage(lng);
  localStorage.setItem(languageKey, lng);
};

const changeUIMode = (isDark: boolean) => {
  localStorage.setItem(uiKey, isDark ? 'dark' : 'light');
  if (isDark) {
    document.body.classList.add('dark');
  } else {
    document.body.classList.remove('dark');
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
      sortCards: false
    };
  },

  getters: {
    versionInfo() {
      return versionInfo;
    }
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
    configure() {
      const storedLng = localStorage.getItem(languageKey) || defaultLanguage;
      setI18nLanguage(storedLng);

      const rawStoredDarkMode = localStorage.getItem(uiKey);
      const storedDarkMode = rawStoredDarkMode
        ? rawStoredDarkMode === 'dark'
        : window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
      changeUIMode(storedDarkMode);

      this.$patch({
        useDarkMode: storedDarkMode,
        language: storedLng,
        showQuickMenu: localStorage.getItem(quickMenuKey) === 'on'
      });
    }
  }
});
