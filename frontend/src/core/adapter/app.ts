import { defineStore } from 'pinia';

import { AppConfig } from '@/config';
import { ConfigService } from '@/core/services/api/Config';
import { RouterService } from '@/core/services/global/Router';
import { Awaiter } from '@/core/services/helper/Awaiter';

import { unwrapBackendError } from '../services/api/Errors';
import { MigrationRunner } from './migrations/MigrationRunner';

const versionInfo = {
  override: AppConfig.VersionOverride,
  version: AppConfig.Version,
  hash: AppConfig.VersionHash
};

export const useRootApp = defineStore('app', {
  state: () => {
    return {
      // global app state
      isOffline: false,
      // versions
      serverVersion: new Awaiter() as string | Awaiter<string>,
      clientVersion: AppConfig.Version,
      // updates
      updateAvailable: false,
      offlineReady: false,
      // eslint-disable-next-line @typescript-eslint/no-empty-function
      _updateFunc: (() => {}) as unknown as (reload: boolean) => Promise<void>
    };
  },
  getters: {
    versionInfo() {
      return versionInfo;
    }
  },
  actions: {
    async configure() {
      const response = await ConfigService.fetchVersion();
      const [version, err] = unwrapBackendError(response);
      if (err && AppConfig.UseBackend) {
        // enable offline mode
        this.isOffline = true;
        console.warn('### zwoo entered offline mode');
        RouterService.getRouter().push('/offline');
      } else if (version !== AppConfig.Version) {
        RouterService.getRouter().push('/invalid-version');
      }

      if (typeof this.serverVersion !== 'string' && typeof response === 'string') {
        this.serverVersion.callback(response);
      }

      MigrationRunner.run(MigrationRunner.lastVersion, this.clientVersion);
    },

    _setUpdateFunc(func: (reload: boolean) => Promise<void>) {
      this._updateFunc = func;
    },
    onOfflineReady() {
      this.offlineReady = true;
    },
    onNeedsRefresh() {
      this.updateAvailable = true;
    },
    updateApp() {
      this._updateFunc(true);
    }
  }
});