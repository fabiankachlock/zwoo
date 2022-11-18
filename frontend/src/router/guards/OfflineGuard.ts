import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

import { useRootApp } from '@/core/adapter/app';
import Logger from '@/core/services/logging/logImport';
import { RouterInterceptor } from '@/router/types';

export class OfflineGuard implements RouterInterceptor {
  private Logger = Logger.RouterGuard.createOne('offline');

  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    if (useRootApp().isOffline) {
      this.Logger.log('enforcing offline mode');
      if (to.fullPath !== '/offline') {
        next('/offline');
      } else {
        next();
      }
      return true;
    } else if (!useRootApp().isOffline && to.fullPath === '/offline') {
      next('/');
      return true;
    } else {
      return false;
    }
  };
}
