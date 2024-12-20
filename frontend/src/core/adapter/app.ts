import { defineStore } from 'pinia';

import { AppConfig } from '@/config';
import { RouterService } from '@/core/global/Router';
import { Awaiter } from '@/core/helper/Awaiter';

import { ApiAdapter } from '../api/ApiAdapter';
import { BackendError } from '../api/ApiError';
import { ClientInfo } from '../api/entities/Misc';
import { GameAdapter } from '../api/GameAdapter';
import { RestApi } from '../api/restapi/RestApi';
import { WasmApi } from '../api/wasmapi/WasmApi';
import { WsGameAdapter } from '../api/wsgame/WsGameAdapter';
import { ZRP_VERSION } from '../domain/zrp/zrpTypes';
import Logger from '../services/logging/logImport';
import { useAuth } from './auth';
import { MigrationRunner } from './migrations/MigrationRunner';
import { SnackBarPosition, useSnackbar } from './snackbar';

type AppEnv = 'offline' | 'online' | 'local';

export const useRootApp = defineStore('app', {
  state: () => {
    return {
      // global app state
      isLoading: true,
      isConfigured: new Awaiter(),
      environment: (AppConfig.DefaultEnv ?? 'online') as AppEnv,
      // versions
      serverVersionMatches: new Awaiter() as boolean | Awaiter<boolean>,
      serverVersion: new Awaiter() as ClientInfo | Awaiter<ClientInfo>,
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
    versionInfo: (state): ClientInfo => {
      return {
        version: AppConfig.VersionOverride || AppConfig.Version,
        hash: AppConfig.VersionHash,
        zrpVersion: ZRP_VERSION,
        mode: state.environment
      };
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
      // if env is locked setup must run static
      if (AppConfig.LockEnv && AppConfig.DefaultEnv) {
        await this.setupLocked();
        MigrationRunner.migrateTo(AppConfig.Version);
        this.isLoading = false;
        this.isConfigured.callback(void 0);
        return;
      }

      // otherwise setup dynamically
      // assume online mode by default
      this.environment = (AppConfig.DefaultEnv as AppEnv) ?? 'online';
      const auth = useAuth();
      const hasLocalLogin = await auth.tryLocalLogin();

      const response = await this.api.checkVersion(this.versionInfo);
      if (response.isError && response.error.code === BackendError.InvalidClient) {
        // backend marked client as invalid
        RouterService.getRouter().push('/invalid-version');
        this._setServerVersion(response.error.problem['version']?.toString() || '');
        this._setServerVersionMatches(false);
      } else if (response.isError && AppConfig.UseBackend) {
        // enable offline mode
        if (!hasLocalLogin) {
          this.environment = 'offline';
          console.warn('### zwoo entered offline mode');
          await auth.applyOfflineConfig();
        }
        this._setServerVersion(this.versionInfo);
        this._setServerVersionMatches(true);
      } else if (response.wasSuccessful) {
        // TODO: check version
        if (!hasLocalLogin) {
          await auth.configure();
        }
        this._setServerVersion(response.data);
        this._setServerVersionMatches(true);
      }

      MigrationRunner.migrateTo(AppConfig.Version);
      this.isLoading = false;
      this.isConfigured.callback(void 0);
    },
    async setupLocked() {
      this.environment = AppConfig.DefaultEnv as AppEnv;
      Logger.warn(`### zwoo statically entered ${this.environment} mode`);
      if (AppConfig.DefaultEnv === 'online') {
        // setup for online mode
        const response = await this.api.checkVersion(this.versionInfo);
        if (response.wasSuccessful) {
          this._setServerVersion(response.data);
          this._setServerVersionMatches(true);
        } else if (response.isError && response.error.code === BackendError.InvalidClient) {
          // backend marked client as invalid
          RouterService.getRouter().push('/invalid-version');
          this._setServerVersion(response.error.problem['version']?.toString() || 'unknown');
          this._setServerVersionMatches(false);
        } else {
          RouterService.getRouter().push('/locked?target=offline');
          this._setServerVersion(this.versionInfo);
          this._setServerVersionMatches(true);
        }
      } else if (AppConfig.DefaultEnv === 'offline') {
        // setup for offline mode
        this._setServerVersion(this.versionInfo);
        this._setServerVersionMatches(true);
        await useAuth().applyOfflineConfig();
      } else if (AppConfig.DefaultEnv === 'local') {
        // setup for local mode
        this._setServerVersion(this.versionInfo);
        this._setServerVersionMatches(true);
        const hasLocalLogin = await useAuth().tryLocalLogin();
        if (!hasLocalLogin) {
          RouterService.getRouter().push('/login-local');
        }
      }
    },
    _setServerVersion(version: ClientInfo) {
      if ('callback' in this.serverVersion) {
        this.serverVersion.callback(version);
        this.serverVersion = version;
      } else {
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
    enterLocalMode(serverUrl: string): boolean {
      if (AppConfig.LockEnv && AppConfig.DefaultEnv !== 'local') {
        // cant switch to online mode
        RouterService.getRouter().push('/locked?target=local');
        return false;
      }

      const wsUrl = serverUrl.replace(/^http/, 'ws');

      this._apiMap.local.api = RestApi(serverUrl, wsUrl);
      this._apiMap.local.realtime = WsGameAdapter(serverUrl, wsUrl);
      this.environment = 'local';
      Logger.warn('### zwoo entered local mode');
      return true;
    }
  }
});
