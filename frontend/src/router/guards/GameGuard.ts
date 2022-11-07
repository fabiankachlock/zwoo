import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

import { useRedirect } from '@/composables/useRedirect';
import { useGameConfig } from '@/core/adapter/game';
import Logger from '@/core/services/logging/logImport';
import { GameRoute } from '@/router/game';
import { RouterInterceptor } from '@/router/types';

export class InGameGuard implements RouterInterceptor {
  static InGameRoutes: string[] = GameRoute.children?.map(child => `${GameRoute.path ?? ''}/${child?.path ?? ''}`) ?? [];
  private Logger = Logger.RouterGuard.createOne('game');

  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    const game = useGameConfig();
    if (InGameGuard.InGameRoutes.includes(to.path)) {
      this.Logger.debug(`${to.fullPath} needs active game`);

      if (!game.inActiveGame) {
        this.Logger.warn(`not allowed to access ${to.fullPath}`);
        const redirect = to.meta['redirect'] as string | boolean | undefined;
        const { createRedirect } = useRedirect();

        if (redirect === true) {
          this.Logger.log(`redirecting to login with loopback `);
          next(`/login?${createRedirect(to.fullPath)}`);
        } else {
          this.Logger.log(`redirecting to ${redirect || '/list'}`);
          next(redirect || '/list');
        }

        return true;
      }
      this.Logger.debug('all fine!');
    } else if (game.inActiveGame) {
      game.tryLeave();
    }
    return false;
  };
}
