import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

import { createRedirect } from '@/composables/useRedirect';
import { useRootApp } from '@/core/adapter/app';
import { useAuth } from '@/core/adapter/auth';
import Logger from '@/core/services/logging/logImport';
import { RouterInterceptor } from '@/router/types';

export class AuthGuard implements RouterInterceptor {
  private Logger = Logger.RouterGuard.createOne('auth');

  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    const auth = useAuth();
    const app = useRootApp();

    if (to.meta['requiresAuth'] === true || to.meta['noAuth'] === true) {
      this.Logger.debug(to.meta['requiresAuth'] === true ? `${to.fullPath} needs auth` : `${to.fullPath} only available without auth`);
      if (!auth.isInitialized) {
        this.Logger.debug('force auth init');
        await auth.askStatus();
      }

      // needs an exception for offline mode, because when offline this user is not officially logged in (e.g. has no account settings etc.)
      if (
        (to.meta['requiresAuth'] === true && !auth.isLoggedIn && app.environment !== 'offline') ||
        (to.meta['noAuth'] === true && auth.isLoggedIn)
      ) {
        this.Logger.warn(`not allowed to access ${to.fullPath}`);
        const redirect = to.meta['redirect'] as string | boolean | undefined;

        if (redirect === true) {
          this.Logger.log(`redirecting to login with loopback `);
          next(`/login?${createRedirect(to.fullPath)}`);
        } else {
          this.Logger.log(`redirecting to ${redirect}`);
          next(redirect || '/');
        }

        return true;
      }
      this.Logger.debug('all fine!');
    }
    return false;
  };
}
