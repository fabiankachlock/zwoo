import { getCurrentWindow } from '@tauri-apps/api/window';
import { defineStore } from 'pinia';

import { AppConfig } from '@//config';
import { CardThemeIdentifier } from '@/core/domain/cards/CardThemeConfig';
import { CardThemeManager } from '@/core/domain/cards/ThemeManager';
import { Logger as _Logger } from '@/core/services/logging/logImport';
import { defaultLanguage, setI18nLanguage, supportedLanguages } from '@/i18n';

import { useApi } from './helper/useApi';

const Logger = _Logger.createOne('config');
const configKey = 'zwoo:config';

export enum ZwooConfigKey {
  Sync = 'sync',
  Language = 'lng',
  UiMode = 'ui',
  UiContrastMode = 'uic',
  QuickMenu = 'qm',
  SortCards = 'sc',
  ShowCardsDetail = 'cd',
  CardsTheme = 'th',
  UserDefaults = 'ud',
  FeedbackChat = 'fc',
  FeedbackSnackbar = 'fs',
  DevSettings = 'dev-settings',
  Logging = 'logging',
  _Version = '#v'
}

export type ZwooConfig = {
  _ignore: Partial<Record<ZwooConfigKey, boolean>>;
  [ZwooConfigKey.Sync]: boolean;
  [ZwooConfigKey.Language]: string;
  [ZwooConfigKey.UiMode]: string;
  [ZwooConfigKey.UiContrastMode]: boolean;
  [ZwooConfigKey.QuickMenu]: boolean;
  [ZwooConfigKey.SortCards]: boolean;
  [ZwooConfigKey.ShowCardsDetail]: boolean;
  [ZwooConfigKey.CardsTheme]: CardThemeIdentifier;
  [ZwooConfigKey.UserDefaults]: string;
  [ZwooConfigKey.FeedbackChat]: string;
  [ZwooConfigKey.FeedbackSnackbar]: string;
  [ZwooConfigKey.DevSettings]: boolean;
  [ZwooConfigKey.Logging]: string;
  [ZwooConfigKey._Version]: string;
};

const DefaultConfig: ZwooConfig = {
  _ignore: {},
  sync: true,
  ui: 'dark',
  uic: false,
  lng: 'en',
  qm: false,
  sc: false,
  cd: false,
  th: {
    name: '__default__',
    variant: '@auto'
  },
  fc: 'all',
  fs: 'none',
  'dev-settings': false,
  logging: '',
  ud: '',
  '#v': AppConfig.Version
};

const changeLanguage = (lng: string) => {
  setI18nLanguage(lng);
};

const changeUIMode = (mode: string) => {
  const isDark = mode === 'dark';
  document.querySelector('body')?.classList.toggle('dark', isDark);
  document.querySelector('body')?.classList.toggle('light', !isDark);
};

const changeUIContrastMode = (isHighContrast: boolean) => {
  document.querySelector('body')?.classList.toggle('highContrast', isHighContrast);
};

const changeFullscreen = (enabled: boolean) => {
  if (enabled) {
    if (AppConfig.IsTauri) {
      getCurrentWindow().setFullscreen(true);
    } else {
      document.documentElement.requestFullscreen();
    }
  } else {
    if (AppConfig.IsTauri) {
      getCurrentWindow().setFullscreen(false);
    } else if (document.fullscreenElement) {
      document.exitFullscreen();
    }
  }
};

export const useConfig = defineStore('config', {
  state: () => {
    return {
      _config: {} as ZwooConfig,
      _isLoggedIn: false,
      useFullScreen: false
    };
  },

  getters: {
    get:
      state =>
      <K extends ZwooConfigKey>(key: K) =>
        state._config[key],
    isSynced:
      state =>
      <K extends ZwooConfigKey>(key: K) =>
        !state._config._ignore[key] && state._config.sync
  },

  actions: {
    set<K extends ZwooConfigKey>(key: K, value: ZwooConfig[K], save = true) {
      this._config[key] = value;

      if (ZwooConfigKey.UiMode === key) {
        changeUIMode(value as string);
      } else if (ZwooConfigKey.UiContrastMode === key) {
        changeUIContrastMode(value as boolean);
      } else if (ZwooConfigKey.Language === key) {
        changeLanguage(value as string);
      } else if (ZwooConfigKey.DevSettings === key && value) {
        localStorage.setItem('zwoo:dev-settings', 'true');
      }

      if (save) {
        this._saveConfig();
      }
    },
    toggleIgnore(key: ZwooConfigKey) {
      this._config._ignore[key] = !this._config._ignore[key];
      if (!this._config._ignore[key]) {
        delete this._config._ignore[key];
      }
      this._saveConfig();
    },
    setFullScreen(enabled: boolean) {
      this.useFullScreen = enabled;
      changeFullscreen(enabled);
    },
    async loadProfile() {
      Logger.info(`loading config for the current user`);
      const config = await useApi().loadUserSettings();
      if (config.wasSuccessful) {
        const parsedConfig = this._deserializeConfig(config.data.settings);
        this.applyConfig(parsedConfig ?? {});
      }
    },
    applyConfig(config: Omit<Partial<ZwooConfig>, '_ignore'>) {
      for (const [key, value] of Object.entries(config)) {
        this.set(key as ZwooConfigKey, value, false);
      }
      if (localStorage.getItem('zwoo:dev-settings') === 'true') {
        this.set(ZwooConfigKey.DevSettings, true);
      }
      this._saveConfig(true);
    },
    deleteLocalChanges() {
      Logger.info(`cleared local config`);
      localStorage.removeItem(configKey);
      this.applyConfig(this._createDefaultConfig());
    },
    async _saveConfig(onlyLocal?: boolean) {
      // dont ignore stuff on local config
      localStorage.setItem(configKey, this._serializeConfig(this._config, true));
      if (this._isLoggedIn && !onlyLocal) {
        // save full config remote
        await useApi().storeUserSettings(this._serializeConfig(this._config, false));
      }
    },
    _serializeConfig(config: ZwooConfig, ignore: boolean): string {
      return config[ZwooConfigKey.Sync]
        ? JSON.stringify(
            (Object.keys(config) as ZwooConfigKey[]).reduce((all, key) => (config._ignore[key] && !ignore ? all : { ...all, [key]: config[key] }), {})
          )
        : JSON.stringify({ [ZwooConfigKey.Sync]: false });
    },
    _deserializeConfig(config: string): Partial<ZwooConfig> | undefined {
      try {
        return JSON.parse(config) as ZwooConfig;
      } catch (err: unknown) {
        Logger.warn(`cant parse config: ${err}`);
        return undefined;
      }
    },
    _createDefaultConfig(): ZwooConfig {
      const settingsToApply = DefaultConfig;
      const userLng = navigator.language.split('-')[0]?.toLowerCase();
      settingsToApply.lng = supportedLanguages.includes(userLng) ? userLng : defaultLanguage;
      settingsToApply.ui = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
      settingsToApply.cd = 'ontouchstart' in document.documentElement;
      return settingsToApply;
    },
    _migrateConfig(config: Partial<ZwooConfig>) {
      if (!config[ZwooConfigKey._Version]) {
        return this._createDefaultConfig();
      }
      // do migrations
      // ...
      // auto migrate:
      // ATTENTION this may migrates config implicitly
      for (const [key, value] of Object.entries(DefaultConfig as Omit<Partial<ZwooConfig>, '_ignore'>)) {
        if (this.get(key as ZwooConfigKey) === undefined) {
          this.set(key as ZwooConfigKey, value, false);
        }
      }
      config['#v'] = AppConfig.Version;
      return config;
    },
    login() {
      if (!this._isLoggedIn) {
        this.loadProfile();
      }
      this._isLoggedIn = true;
    },
    logout() {
      this._isLoggedIn = false;
      this.deleteLocalChanges();
    },
    configure() {
      const storedConfig = localStorage.getItem(configKey) ?? '';
      const parsedConfig = this._deserializeConfig(storedConfig);
      if (parsedConfig) {
        this.applyConfig(this._migrateConfig(parsedConfig));
      } else {
        this.applyConfig(this._createDefaultConfig());
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
