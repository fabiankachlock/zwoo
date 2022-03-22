import { useGameConfig } from '@/core/adapter/game';
import { GameRoute } from '@/router/game';
import { RouterInterceptor } from '@/router/types';
import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

export class InGameGuard implements RouterInterceptor {
  static InGameRoutes: string[] = GameRoute.children?.map(child => child?.path ?? '') ?? [];

  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    if (InGameGuard.InGameRoutes.includes(to.path)) {
      const game = useGameConfig();
      if (!game.inActiveGame) {
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
