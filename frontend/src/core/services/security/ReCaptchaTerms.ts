import { RouterInterceptor } from '@/router/types';
import { RouteLocationNormalized } from 'vue-router';

export class ReCaptchaTermsVisibilityManager {
  private static instance = new ReCaptchaTermsVisibilityManager();

  private getBadgeElement: () => Element | null = () => null;

  static init = (): void => {
    ReCaptchaTermsVisibilityManager.instance.getBadgeElement = () => document.querySelector('.grecaptcha-badge');
  };

  static updateState = (): void => {
    if (ReCaptchaTermsRouteInterceptor.matchesRoute(window.location.pathname)) {
      ReCaptchaTermsVisibilityManager.showBadge();
      return;
    }
    ReCaptchaTermsVisibilityManager.hideBadge();
  };

  static showBadge = (): void => {
    ReCaptchaTermsVisibilityManager.instance.getBadgeElement()?.classList.add('visible');
  };

  static hideBadge = (): void => {
    ReCaptchaTermsVisibilityManager.instance.getBadgeElement()?.classList.remove('visible');
  };
}

export class ReCaptchaTermsRouteInterceptor implements RouterInterceptor {
  static VisibleRouts = ['/login', '/create-account'];

  static matchesRoute = (route: string): boolean => {
    for (const r of ReCaptchaTermsRouteInterceptor.VisibleRouts) {
      if (route.startsWith(r)) return true;
    }
    return false;
  };

  afterEachAsync = async (_from: RouteLocationNormalized, current: RouteLocationNormalized): Promise<void> => {
    if (ReCaptchaTermsRouteInterceptor.matchesRoute(current.fullPath)) {
      ReCaptchaTermsVisibilityManager.showBadge();
    } else {
      ReCaptchaTermsVisibilityManager.hideBadge();
    }
  };
}

export default ReCaptchaTermsRouteInterceptor;
