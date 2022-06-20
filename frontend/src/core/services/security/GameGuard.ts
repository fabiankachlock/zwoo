import { useGameConfig } from '@/core/adapter/game';
import { GameRoute } from '@/router/game';
// import { GameRoute } from '@/router/game';
import { RouterInterceptor } from '@/router/types';
import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';
import Logger from '../logging/logImport';

export class InGameGuard implements RouterInterceptor {
  static InGameRoutes: string[] = GameRoute.children?.map(child => `${GameRoute.path ?? ''}/${child?.path ?? ''}`) ?? [];
  private Logger = Logger.RouterGuard.createOne('game');

  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    if (InGameGuard.InGameRoutes.includes(to.path)) {
      this.Logger.debug(`${to.fullPath} needs active game`);

      const game = useGameConfig();
      if (!game.inActiveGame) {
        this.Logger.warn(`not allowed to access ${to.fullPath}`);
        const redirect = to.meta['redirect'] as string | boolean | undefined;

        if (redirect === true) {
          this.Logger.log(`redirecting to login with loopback `);
          next('/login?redirect=' + to.fullPath);
        } else {
          this.Logger.log(`redirecting to ${redirect || '/list'}`);
          next(redirect || '/list');
        }

        return true;
      }
      this.Logger.debug('all fine!');
    }
    return false;
  };
}
