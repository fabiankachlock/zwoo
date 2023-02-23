import { defineStore } from 'pinia';

import { AppConfig } from '@//config';
import { AccountService } from '@/core/api/restapi/Account';
import { CardThemeIdentifier } from '@/core/domain/cards/CardThemeConfig';
import { CardThemeManager } from '@/core/domain/cards/ThemeManager';
import { Logger as _Logger } from '@/core/services/logging/logImport';
import { defaultLanguage, setI18nLanguage, supportedLanguages } from '@/i18n';

const Logger = _Logger.createOne('config');
const configKey = 'zwoo:config';

export enum ZwooConfigKey {
  Sync = 'sync',
  Language = 'lng',
  UiMode = 'ui',
  QuickMenu = 'qm',
  SortCards = 'sc',
  ShowCardsDetail = 'cd',
  CardsTheme = 'th',
  UserDefaults = 'ud',
  DevSettings = 'dev-settings',
  Logging = 'logging',
  _Version = '#v'
}

export type ZwooConfig = {
  _ignore: Partial<Record<ZwooConfigKey, boolean>>;
  [ZwooConfigKey.Sync]: boolean;
  [ZwooConfigKey.Language]: string;
  [ZwooConfigKey.UiMode]: string;
  [ZwooConfigKey.QuickMenu]: boolean;
  [ZwooConfigKey.SortCards]: boolean;
  [ZwooConfigKey.ShowCardsDetail]: boolean;
  [ZwooConfigKey.CardsTheme]: CardThemeIdentifier;
  [ZwooConfigKey.UserDefaults]: string;
  [ZwooConfigKey.DevSettings]: boolean;
  [ZwooConfigKey.Logging]: string;
  [ZwooConfigKey._Version]: string;
};

const DefaultConfig: ZwooConfig = {
  _ignore: {},
  sync: true,
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
  ud: '',
  '#v': AppConfig.Version
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
      } else if (ZwooConfigKey.Language === key) {
        changeLanguage(value as string);
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
      const config = await AccountService.loadSettings();
      if (config) {
        const parsedConfig = this._deserializeConfig(config);
        this.applyConfig(parsedConfig ?? {});
      }
    },
    applyConfig(config: Omit<Partial<ZwooConfig>, '_ignore'>) {
      for (const [key, value] of Object.entries(config)) {
        this.set(key as ZwooConfigKey, value, false);
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
        await AccountService.storeSettings(this._serializeConfig(this._config, false));
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
