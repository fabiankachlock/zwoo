/* eslint-disable */
// @ts-nocheck
import { useConfig } from '@/core/adapter/config';
import { ZwooConfigKey } from '@/core/adapter/config';
import { defaultLanguage, supportedLanguages } from '@/i18n';

export default {
  up() {
    const userLng = navigator.language.split('-')[0]?.toLowerCase();
    const uiLng = supportedLanguages.includes(userLng) ? userLng : defaultLanguage;
    useConfig().set(ZwooConfigKey.Language, uiLng);
    return;
  },
  down() {
    return;
  }
};
