import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

import { useCookies } from '@/core/adapter/cookies';
import { RouterInterceptor } from '@/router/types';

import Logger from '../logging/logImport';

export class CookieGuard implements RouterInterceptor {
  private static Logger = Logger.RouterGuard.createOne('cookies');
  static RecaptchaCookieRoutes = ['/login', '/create-account'];

  static matchesRoute = (route: string): boolean => {
    for (const r of CookieGuard.RecaptchaCookieRoutes) {
      if (route.startsWith(r)) return true;
    }
    return false;
  };

  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    const needsCookie = CookieGuard.matchesRoute(to.fullPath);

    if (needsCookie) {
      CookieGuard.Logger.debug('needs cookie approval');
      const cookies = useCookies();
      if (!cookies.cookies.recaptcha) {
        CookieGuard.Logger.log('insufficient cookie settings');
        next('/missing-cookies?redirect=' + to.fullPath);
      }
      CookieGuard.Logger.debug('all fine!');
    }
    return false;
  };
}
