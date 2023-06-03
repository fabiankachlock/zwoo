import { defineStore } from 'pinia';

import { AppConfig } from '@/config';
import { useRuntimeConfig } from '@/core/runtimeConfig';

import { unwrapBackendError } from '../api/ApiError';
import { CaptchaResponse } from '../api/entities/Captcha';
import { ReCaptchaTermsVisibilityManager } from '../services/captcha/ReCaptchaTermsVisibilityManager';
import { useApi } from './helper/useApi';

export const useCaptcha = defineStore('captcha', {
  state: () => ({
    isReady: false,
    loaded: false
  }),
  actions: {
    whenLoaded() {
      ReCaptchaTermsVisibilityManager.init();
      ReCaptchaTermsVisibilityManager.updateState();
      this.isReady = true;
    },
    async _loadScript() {
      if (this.loaded) return;
      this.loaded = true;
      const siteKey = await useRuntimeConfig().recaptchaKey;
      const scriptTag = document.createElement('script');
      scriptTag.setAttribute('src', 'https://www.google.com/recaptcha/api.js?render=' + siteKey);
      document.body.appendChild(scriptTag);

      scriptTag.onload = () => {
        grecaptcha.ready(() => this.whenLoaded());
      };
    },
    async check(token: string): Promise<CaptchaResponse | undefined> {
      if (!AppConfig.UseBackend) {
        return Promise.resolve({
          success: true,
          score: 1
        });
      }

      const [result] = unwrapBackendError(await useApi().verifyCaptchaToken(token));
      return result;
    },
    async performCheck(): Promise<CaptchaResponse | undefined> {
      if (!AppConfig.UseBackend) {
        return Promise.resolve({
          success: true,
          score: 1
        });
      }

      if (this.isReady) {
        const siteKey = await useRuntimeConfig().recaptchaKey;
        const token = await grecaptcha.execute(siteKey, {
          action: 'login'
        });
        const [result] = unwrapBackendError(await useApi().verifyCaptchaToken(token));
        return result;
      }
      return Promise.resolve({
        success: true,
        score: 1
      });
    }
  }
});
