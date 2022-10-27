import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

import { useConfig } from '@/core/adapter/config';
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

    const config = useConfig();
    let version: string;

    if (typeof config.serverVersion !== 'string') {
      version = await config.serverVersion.promise;
    } else {
      version = config.serverVersion;
    }
    this.Logger.debug(`version lock initialized; client: ${config.clientVersion}, server: ${version}`);
    this.versionMatches = config.clientVersion === version;
    this.Logger.debug(`version match: ${this.versionMatches}`);
    return this.beforeEach(to, _from, next);
  };
}
