import { useAuth } from '@/core/adapter/auth';
import { RouterInterceptor } from '@/router/types';
import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

export class AuthGuard implements RouterInterceptor {
  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    const auth = useAuth();

    if (to.meta['requiresAuth'] === true) {
      if (!auth.isInitialized) {
        await auth.askStatus();
      }

      if (!auth.isLoggedIn) {
        const redirect = to.meta['redirect'] as string | boolean | undefined;

        if (redirect === true) {
          next('/login?redirect=' + to.fullPath);
        } else {
          next(redirect || '/');
        }

        return true;
      }
    }
    return false;
  };
}

