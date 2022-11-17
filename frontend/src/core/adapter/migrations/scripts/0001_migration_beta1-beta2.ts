// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-nocheck
import { useConfig } from '@/core/adapter/config';
import { defaultLanguage, supportedLanguages } from '@/i18n';

export default {
  up() {
    const userLng = navigator.language.split('-')[0]?.toLowerCase();
    const uiLng = supportedLanguages.includes(userLng) ? userLng : defaultLanguage;
    useConfig().setLanguage(uiLng);
    return;
  },
  down() {
    return;
  }
};
