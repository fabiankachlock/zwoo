import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

import { useAuth } from '@/core/adapter/auth';
import Logger from '@/core/services/logging/logImport';
import { RouterInterceptor } from '@/router/types';

export class AuthGuard implements RouterInterceptor {
  private Logger = Logger.RouterGuard.createOne('auth');

  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    const auth = useAuth();

    if (to.meta['requiresAuth'] === true || to.meta['noAuth'] === true) {
      this.Logger.debug(to.meta['requiresAuth'] === true ? `${to.fullPath} needs auth` : `${to.fullPath} only available without auth`);
      if (!auth.isInitialized) {
        this.Logger.debug('force auth init');
        await auth.askStatus();
      }

      if ((to.meta['requiresAuth'] === true && !auth.isLoggedIn) || (to.meta['noAuth'] === true && auth.isLoggedIn)) {
        this.Logger.warn(`not allowed to access ${to.fullPath}`);
        const redirect = to.meta['redirect'] as string | boolean | undefined;

        if (redirect === true) {
          this.Logger.log(`redirecting to login with loopback `);
          next('/login?redirect=' + to.fullPath);
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
