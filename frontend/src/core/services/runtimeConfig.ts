import { defineStore } from 'pinia';

import { WrappedFetch } from './api/FetchWrapper';
import { Awaiter } from './helper/Awaiter';

const configKey = 'zwoo:runtimeConfig';

type ConfigType = {
  config: {
    recaptcha: string;
    logrushServer?: string;
  };
  features: {
    beta: boolean;
  };
};

export const useRuntimeConfig = defineStore('runtime-config', {
  state: () => ({
    _isBeta: new Awaiter<boolean>() as boolean | Awaiter<boolean>,
    _recaptchaKey: new Awaiter<string>() as string | Awaiter<string>,
    _logrushServer: new Awaiter<string | undefined>() as string | undefined | Awaiter<string>
  }),

  getters: {
    isBeta: state => (typeof state._isBeta === 'object' ? state._isBeta.promise : Promise.resolve(state._isBeta)),
    recaptchaKey: state => (typeof state._recaptchaKey === 'object' ? state._recaptchaKey.promise : Promise.resolve(state._recaptchaKey)),
    logrushServer: state => (typeof state._logrushServer === 'object' ? state._logrushServer.promise : Promise.resolve(state._logrushServer))
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
        if (typeof this._logrushServer === 'object') {
          this._logrushServer.callback(parsed.config.logrushServer);
        } else {
          this._logrushServer = parsed.config.logrushServer;
        }
      } catch {
        return;
      }
    },
    async configure() {
      const config = await WrappedFetch<string>('config.json', {
        useBackend: true,
        responseOptions: {
          decodeJson: false
        }
      });
      if (config.data) {
        this._loadConfig(config.data);
        localStorage.setItem(configKey, config.data);
      } else if (config.error) {
        const storedConfig = localStorage.getItem(configKey);
        if (storedConfig) {
          this._loadConfig(storedConfig);
        }
      }
    }
  }
});
