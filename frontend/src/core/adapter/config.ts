import { defineStore } from 'pinia';

import { Logger as _Logger } from '@/core/services/logging/logImport';
import { defaultLanguage, setI18nLanguage, supportedLanguages } from '@/i18n';

import { CardThemeIdentifier } from '../services/cards/CardThemeConfig';
import { CardThemeManager } from '../services/cards/ThemeManager';

const Logger = _Logger.createOne('config');
const configKey = 'zwoo:config';

export enum ZwooConfigKey {
  Language = 'lng',
  UiMode = 'ui',
  QuickMenu = 'qm',
  SortCards = 'sc',
  ShowCardsDetail = 'cd',
  CardsTheme = 'th',
  UserDefaults = 'ud',
  DevSettings = 'dev-settings',
  Logging = 'logging'
}

export type ZwooConfig = {
  [ZwooConfigKey.Language]: string;
  [ZwooConfigKey.UiMode]: string;
  [ZwooConfigKey.QuickMenu]: boolean;
  [ZwooConfigKey.SortCards]: boolean;
  [ZwooConfigKey.ShowCardsDetail]: boolean;
  [ZwooConfigKey.CardsTheme]: CardThemeIdentifier;
  [ZwooConfigKey.UserDefaults]: string;
  [ZwooConfigKey.DevSettings]: boolean;
  [ZwooConfigKey.Logging]: string;
};

const DefaultConfig: ZwooConfig = {
  ui: 'dark',
  lng: 'en',
  qm: false,
  sc: false,
  cd: false,
  th: {
    name: '__default__',
    variant: '@auto'
  },
  'dev-settings': false,
  logging: '',
  ud: ''
};

const changeLanguage = (lng: string) => {
  setI18nLanguage(lng);
};

const changeUIMode = (mode: string) => {
  const isDark = mode === 'dark';
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
      _config: {} as ZwooConfig,
      useFullScreen: false
    };
  },

  getters: {
    get:
      state =>
      <K extends ZwooConfigKey>(key: K) =>
        state._config[key]
  },

  actions: {
    set<K extends ZwooConfigKey>(key: K, value: ZwooConfig[K], save = true) {
      this._config[key] = value;

      if (ZwooConfigKey.UiMode === key) {
        changeUIMode(value as string);
      } else if (ZwooConfigKey.Language === key) {
        changeLanguage(value as string);
      }

      if (save) {
        localStorage.setItem(configKey, this._serializeConfig(this._config));
      }
    },
    setFullScreen(enabled: boolean) {
      this.useFullScreen = enabled;
      changeFullscreen(enabled);
    },
    loadProfile(key: string) {
      Logger.info(`loading config for ${key}`);
      // fetch config
      // deserialize config
      // apply
      this.applyConfig(DefaultConfig);
    },
    applyConfig(config: Partial<ZwooConfig>) {
      for (const [key, value] of Object.entries(config)) {
        this.set(key as ZwooConfigKey, value, false);
      }
      localStorage.setItem(configKey, this._serializeConfig(this._config));
    },
    deleteLocalChanges() {
      Logger.info(`cleared local config`);
      localStorage.removeItem(configKey);
      this.applyConfig(DefaultConfig);
    },
    _serializeConfig(config: ZwooConfig): string {
      return JSON.stringify(config);
    },
    _deserializeConfig(config: string): Partial<ZwooConfig> | undefined {
      try {
        return JSON.parse(config) as ZwooConfig;
      } catch (err: unknown) {
        Logger.warn(`cant parse config: ${err}`);
        return undefined;
      }
    },
    configure() {
      const storedConfig = localStorage.getItem(configKey) ?? '';
      const parsedConfig = this._deserializeConfig(storedConfig);
      if (parsedConfig) {
        this.applyConfig(parsedConfig);
      } else {
        const settingsToApply = DefaultConfig;
        const userLng = navigator.language.split('-')[0]?.toLowerCase();
        settingsToApply.lng = supportedLanguages.includes(userLng) ? userLng : defaultLanguage;
        settingsToApply.ui = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        settingsToApply.cd = 'ontouchstart' in document.documentElement;
        this.applyConfig(settingsToApply);
      }
      this.asyncSetup();
    },
    async asyncSetup() {
      const storedTheme = this._config.th;
      if (!storedTheme) {
        const theme = await CardThemeManager.global.getDefaultTheme();
        this.set(ZwooConfigKey.CardsTheme, theme);
      }
    }
  }
});
