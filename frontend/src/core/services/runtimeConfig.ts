import { defineStore } from 'pinia';

import { WrappedFetch } from './api/FetchWrapper';
import { Awaiter } from './helper/Awaiter';

const configKey = 'zwoo:runtimeConfig';

type ConfigType = {
  config: {
    recaptcha: string;
  };
  features: {
    beta: boolean;
  };
};

export const useRuntimeConfig = defineStore('runtime-config', {
  state: () => ({
    _isBeta: new Awaiter<boolean>() as boolean | Awaiter<boolean>,
    _recaptchaKey: new Awaiter<string>() as string | Awaiter<string>
  }),

  getters: {
    isBeta: state => (typeof state._isBeta === 'object' ? state._isBeta.promise : Promise.resolve(state._isBeta)),
    recaptchaKey: state => (typeof state._recaptchaKey === 'object' ? state._recaptchaKey.promise : Promise.resolve(state._recaptchaKey))
  },

  actions: {
    _loadConfig(config: string) {
      try {
        const parsed = JSON.parse(config) as ConfigType;
        if (typeof this._isBeta === 'object') {
          this._isBeta.callback(parsed.features.beta);
        } else {
          this._isBeta = parsed.features.beta;
        }
        if (typeof this._recaptchaKey === 'object') {
          this._recaptchaKey.callback(parsed.config.recaptcha);
        } else {
          this._recaptchaKey = parsed.config.recaptcha;
        }
      } catch {
        return;
      }
    },
    async configure() {
      const storedConfig = localStorage.getItem(configKey);
      if (storedConfig) {
        this._loadConfig(storedConfig);
      } else {
        const config = await WrappedFetch<string>('config.json', {
          useBackend: true,
          responseOptions: {
            decodeJson: false
          }
        });
        if (config.data) {
          this._loadConfig(config.data);
          localStorage.setItem(configKey, config.data);
        }
      }
    }
  }
});
