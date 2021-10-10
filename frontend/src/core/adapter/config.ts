import i18n, { defaultLanguage } from '@/i18n';
import { defineStore } from 'pinia';

const languageKey = 'zwoo:lng';
const uiKey = 'zwoo:ui';
const quickMenuKey = 'zwoo:qm';

const changeLanguage = (lng: string) => {
  i18n.global.locale.value = lng;
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
      showQuickMenu: false
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
    configure() {
      const storedLng = localStorage.getItem(languageKey) || defaultLanguage;
      i18n.global.locale.value = storedLng;

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
