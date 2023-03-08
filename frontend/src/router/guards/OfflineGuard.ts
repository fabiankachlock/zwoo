import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

import { useRootApp } from '@/core/adapter/app';
import Logger from '@/core/services/logging/logImport';
import { RouterInterceptor } from '@/router/types';

export class OfflineGuard implements RouterInterceptor {
  private Logger = Logger.RouterGuard.createOne('offline');

  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    const isOffline = useRootApp().environment === 'offline';
    if (isOffline) {
      this.Logger.log('enforcing offline mode');
      if (to.meta['onlineOnly']) {
        if (to.meta['offlineRedirect']) {
          next(to.meta['offlineRedirect']);
        } else {
          next('/offline');
        }
      } else {
        next();
      }
    } else {
      if (to.meta['offlineOnly']) {
        if (to.meta['onlineRedirect']) {
          next(to.meta['onlineRedirect']);
        } else {
          next('/landing');
        }
      } else {
        next();
      }
    }
    return true;
  };
}
