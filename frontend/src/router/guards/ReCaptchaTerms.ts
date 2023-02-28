import { RouteLocationNormalized } from 'vue-router';

import { ReCaptchaTermsVisibilityManager } from '@/core/services/captcha/ReCaptchaTermsVisibilityManager';
import { RouterInterceptor } from '@/router/types';

export class ReCaptchaTermsRouteInterceptor implements RouterInterceptor {
  static VisibleRouts = ['/login', '/create-account', '/request-password-reset', '/reset-password', '/contact'];

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
