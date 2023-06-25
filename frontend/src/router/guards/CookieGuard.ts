import { RouteLocationNormalized } from 'vue-router';

// import { createRedirect } from '@/composables/useRedirect';
// import { useCookies } from '@/core/adapter/cookies';
import Logger from '@/core/services/logging/logImport';
import { RouterInterceptor } from '@/router/types';

export class CookieGuard implements RouterInterceptor {
  private static Logger = Logger.RouterGuard.createOne('cookies');
  static RecaptchaCookieRoutes = ['/login', '/create-account'];

  static matchesRoute = (route: string): boolean => {
    for (const r of CookieGuard.RecaptchaCookieRoutes) {
      if (route.startsWith(r)) return true;
    }
    return false;
  };

  beforeEach = async (to: RouteLocationNormalized): Promise<boolean> => {
    const needsCookie = CookieGuard.matchesRoute(to.fullPath);

    if (needsCookie) {
      CookieGuard.Logger.debug('needs cookie approval');
      // const cookies = useCookies();
      // if (!cookies.cookies.recaptcha) {
      //   CookieGuard.Logger.log('insufficient cookie settings');
      //   next(`/missing-cookies?${createRedirect(to.fullPath)}`);
      // }
      CookieGuard.Logger.debug('all fine!');
    }
    return false;
  };
}
