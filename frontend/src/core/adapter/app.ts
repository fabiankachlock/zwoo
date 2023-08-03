import { defineStore } from 'pinia';

import { AppConfig } from '@/config';
import { unwrapBackendError } from '@/core/api/ApiError';
import { RouterService } from '@/core/global/Router';
import { Awaiter } from '@/core/helper/Awaiter';

import { ApiAdapter } from '../api/ApiAdapter';
import { GameAdapter } from '../api/GameAdapter';
import { RestApi } from '../api/restapi/RestApi';
import { WasmApi } from '../api/wasmapi/WasmApi';
import { WsGameAdapter } from '../api/wsgame/WsGameAdapter';
import { useAuth } from './auth';
import { MigrationRunner } from './migrations/MigrationRunner';
import { SnackBarPosition, useSnackbar } from './snackbar';

type AppEnv = 'offline' | 'online';

const versionInfo = {
  override: AppConfig.VersionOverride,
  version: AppConfig.Version,
  hash: AppConfig.VersionHash
};

const apiMap: Record<AppEnv, { api: ApiAdapter; realtime: GameAdapter }> = {
  online: {
    api: RestApi,
    realtime: WsGameAdapter
  },
  offline: {
    api: WasmApi,
    realtime: WasmApi
  }
};

export const useRootApp = defineStore('app', {
  state: () => {
    return {
      // global app state
      isLoading: true,
      environment: 'online' as AppEnv,
      // versions
      serverVersion: new Awaiter() as string | Awaiter<string>,
      clientVersion: AppConfig.VersionOverride || AppConfig.Version,
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
    },
    api: state => {
      return apiMap[state.environment].api;
    },
    realtimeApi: state => {
      return apiMap[state.environment].realtime;
    }
  },
  actions: {
    async configure() {
      const response = await this.api.loadVersion();
      const [version, err] = unwrapBackendError(response);
      if (err && AppConfig.UseBackend) {
        // enable offline mode
        this.environment = 'offline';
        console.warn('### zwoo entered offline mode');
        useAuth().applyOfflineConfig();
        RouterService.getRouter().push('/offline');
      } else if (version !== AppConfig.Version) {
        RouterService.getRouter().push('/invalid-version');
      }

      if (typeof response === 'string') {
        if (typeof this.serverVersion === 'string') {
          this.serverVersion = response;
        } else {
          this.serverVersion.callback(response);
          this.serverVersion = response;
        }
      } else {
        this.serverVersion = AppConfig.Version;
      }

      MigrationRunner.run(MigrationRunner.lastVersion, AppConfig.Version);
      this.isLoading = false;
    },

    _setUpdateFunc(func: (reload: boolean) => Promise<void>) {
      this._updateFunc = func;
    },
    onOfflineReady() {
      this.offlineReady = true;
      const snackbar = useSnackbar();
      snackbar.pushMessage({
        message: 'snackbar.offlineReady',
        needsTranslation: true,
        position: SnackBarPosition.BottomLeft,
        color: 'primary'
      });
    },
    onNeedsRefresh() {
      this.updateAvailable = true;
    },
    updateApp() {
      this._updateFunc(true);
    }
  }
});
