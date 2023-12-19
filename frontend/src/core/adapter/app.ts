import { defineStore } from 'pinia';

import { AppConfig } from '@/config';
import { RouterService } from '@/core/global/Router';
import { Awaiter } from '@/core/helper/Awaiter';

import { ApiAdapter } from '../api/ApiAdapter';
import { BackendError } from '../api/ApiError';
import { GameAdapter } from '../api/GameAdapter';
import { RestApi } from '../api/restapi/RestApi';
import { WasmApi } from '../api/wasmapi/WasmApi';
import { WsGameAdapter } from '../api/wsgame/WsGameAdapter';
import { useAuth } from './auth';
import { MigrationRunner } from './migrations/MigrationRunner';
import { SnackBarPosition, useSnackbar } from './snackbar';

type AppEnv = 'offline' | 'online' | 'local';

const versionInfo = {
  override: AppConfig.VersionOverride,
  version: AppConfig.Version,
  hash: AppConfig.VersionHash
};

export const useRootApp = defineStore('app', {
  state: () => {
    return {
      // global app state
      isLoading: true,
      environment: (AppConfig.DefaultEnv ?? 'online') as AppEnv,
      // versions
      serverVersionMatches: new Awaiter() as boolean | Awaiter<boolean>,
      serverVersion: new Awaiter() as string | Awaiter<string>,
      clientVersion: AppConfig.VersionOverride || AppConfig.Version,
      // updates
      updateAvailable: false,
      offlineReady: false,
      // eslint-disable-next-line @typescript-eslint/no-empty-function
      _updateFunc: (() => {}) as unknown as (reload: boolean) => Promise<void>,
      _apiMap: {
        online: {
          api: RestApi(AppConfig.ApiUrl, AppConfig.WsUrl),
          realtime: WsGameAdapter(AppConfig.ApiUrl, AppConfig.WsUrl)
        },
        offline: {
          api: WasmApi,
          realtime: WasmApi
        },
        local: {
          api: RestApi(AppConfig.ApiUrl, AppConfig.WsUrl),
          realtime: WsGameAdapter(AppConfig.ApiUrl, AppConfig.WsUrl)
        }
      } as Record<AppEnv, { api: ApiAdapter; realtime: GameAdapter }>
    };
  },
  getters: {
    versionInfo() {
      return versionInfo;
    },
    api: state => {
      return state._apiMap[state.environment].api;
    },
    realtimeApi: state => {
      return state._apiMap[state.environment].realtime;
    }
  },
  actions: {
    async configure() {
      const response = await this.api.checkVersion(AppConfig.Version, '');
      if (response.isError && response.error.code === BackendError.InvalidClient) {
        // backend marked client as invalid
        RouterService.getRouter().push('/invalid-version');
        this._setServerVersion(response.error.problem['version']?.toString() || '');
        this._setServerVersionMatches(false);
      } else if (response.isError && AppConfig.UseBackend) {
        // enable offline mode
        this.environment = 'offline';
        console.warn('### zwoo entered offline mode');
        useAuth().applyOfflineConfig();
        RouterService.getRouter().push(window.location.pathname);
        this._setServerVersion(this.clientVersion);
        this._setServerVersionMatches(true);
      } else if (response.wasSuccessful) {
        // TODO: check version
        this._setServerVersion(response.data.version);
        this._setServerVersionMatches(true);
      }

      MigrationRunner.migrateTo(AppConfig.Version);
      this.isLoading = false;
    },
    _setServerVersion(version: string) {
      if (typeof this.serverVersion === 'string') {
        this.serverVersion = version;
      } else {
        this.serverVersion.callback(version);
        this.serverVersion = version;
      }
    },
    _setServerVersionMatches(matches: boolean) {
      if (typeof this.serverVersionMatches === 'boolean') {
        this.serverVersionMatches = matches;
      } else {
        this.serverVersionMatches.callback(matches);
        this.serverVersionMatches = matches;
      }
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
    },
    enterLocalMode(serverUrl: string) {
      // TODO: handle ws url
      this._apiMap.local.api = RestApi(serverUrl, AppConfig.WsUrl);
      this.environment = 'local';
    }
  }
});
