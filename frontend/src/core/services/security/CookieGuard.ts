import { RouterInterceptor } from '@/router/types';
import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';
import { useCookies } from '@/core/adapter/cookies';

export class CookieGuard implements RouterInterceptor {
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
      const cookies = useCookies();
      console.log(cookies.cookies);
      if (!cookies.cookies.recaptcha) {
        next('/missing-cookies');
      }
    }
    return false;
  };
}
