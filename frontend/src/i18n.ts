import { nextTick } from 'vue';
import { createI18n } from 'vue-i18n';

import { AppConfig } from './config';

export const supportedLanguages = ['en', 'de'];
export const defaultLanguage = 'en';

const _i18n = createI18n({
  legacy: false,
  globalInjection: false,
  supportedLanguages: supportedLanguages,
  locale: AppConfig.I18nLocale || 'en',
  fallbackLocale: AppConfig.I18nFallbackLocale || 'en'
});

export const setI18nLanguage = (locale: string): void => {
  _i18n.global.locale.value = locale;

  if (!_i18n.global.availableLocales.includes(locale)) {
    loadLocaleMessages(locale);
  }

  document.querySelector('html')?.setAttribute('lang', locale);
};

export const loadLocaleMessages = async (locale: string): Promise<void> => {
  const messages = await import(`./locales/${locale}.json`);

  _i18n.global.setLocaleMessage(locale, messages.default);

  return nextTick();
};

// intial setup
loadLocaleMessages('en');
setI18nLanguage(defaultLanguage);

const customizablePaths = ['cardThemes.variant', 'cardThemes.description'];

_i18n.global.setMissingHandler((_, key) => {
  for (const path of customizablePaths) {
    if (key.startsWith(path)) {
      return key.substring(path.length + 1);
    }
  }
});

export default _i18n;
export const I18nInstance = _i18n.global;
