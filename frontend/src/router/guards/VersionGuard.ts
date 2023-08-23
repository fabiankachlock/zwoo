import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

import { useRootApp } from '@/core/adapter/app';
import Logger from '@/core/services/logging/logImport';
import { RouterInterceptor } from '@/router/types';

export class VersionGuard implements RouterInterceptor {
  private Logger = Logger.RouterGuard.createOne('version');

  private versionMatches: boolean | undefined = undefined;

  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    if (this.versionMatches === true) {
      if (to.fullPath == '/invalid-version') {
        next('/');
        return true;
      }
      return false;
    } else if (this.versionMatches === false) {
      if ((to.fullPath.startsWith('/settings') && to.query['reason'] === 'update') || to.fullPath === '/invalid-version') {
        return false;
      }
      next('/invalid-version');
      return true;
    }

    const app = useRootApp();
    let version: string;

    if (typeof app.serverVersion !== 'string') {
      version = await app.serverVersion.promise;
    } else {
      version = app.serverVersion;
    }
    this.Logger.debug(`version lock initialized; client: ${app.clientVersion}, server: ${version}`);
    this.versionMatches = app.versionInfo.version === version || app.versionInfo.override === version;
    this.Logger.debug(`version match: ${this.versionMatches}`);
    return this.beforeEach(to, _from, next);
  };
}
