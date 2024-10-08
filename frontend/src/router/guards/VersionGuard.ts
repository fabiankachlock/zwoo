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
    if (typeof app.serverVersionMatches !== 'boolean') {
      this.versionMatches = await app.serverVersionMatches.promise;
    } else {
      this.versionMatches = app.serverVersionMatches;
    }

    this.Logger.debug(`version lock initialized; client: ${app.versionInfo.version}, server: ${this.versionMatches ? 'compatible' : 'incompatible'}`);
    this.Logger.debug(`version match: ${this.versionMatches}`);
    return this.beforeEach(to, _from, next);
  };
}
