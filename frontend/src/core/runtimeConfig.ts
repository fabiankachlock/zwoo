import { defineStore } from 'pinia';

import { Awaiter } from '@/core/helper/Awaiter';

import { useRootApp } from './adapter/app';

const configKey = 'zwoo:runtimeConfig';

type ConfigType = {
  config: {
    captcha: string;
    logrushServer?: string;
  };
  features: {
    beta: boolean;
  };
};

export const useRuntimeConfig = defineStore('runtime-config', {
  state: () => ({
    _isBeta: new Awaiter<boolean>() as boolean | Awaiter<boolean>,
    _captchaKey: new Awaiter<string>() as string | Awaiter<string>,
    _logrushServer: new Awaiter<string | undefined>() as string | undefined | Awaiter<string>
  }),

  getters: {
    isBeta: state => (typeof state._isBeta === 'object' ? state._isBeta.promise : Promise.resolve(state._isBeta)),
    captchaKey: state => (typeof state._captchaKey === 'object' ? state._captchaKey.promise : Promise.resolve(state._captchaKey)),
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
        if (typeof this._captchaKey === 'object') {
          this._captchaKey.callback(parsed.config.captcha);
        } else {
          this._captchaKey = parsed.config.captcha;
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
      const config = await useRootApp().api.fetchRaw<string>('config.json', {
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
