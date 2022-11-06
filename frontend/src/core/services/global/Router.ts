import { NavigationFailure, RouteLocationRaw } from 'vue-router';

export interface ClientRouter {
  push(to: RouteLocationRaw): Promise<NavigationFailure | void | undefined>;
  replace(to: RouteLocationRaw): Promise<NavigationFailure | void | undefined>;
  back(): ReturnType<ClientRouter['go']>;
  forward(): ReturnType<ClientRouter['go']>;
  go(delta: number): void;
}

export class RouterService {
  private static router: ClientRouter;

  public static registerRouter(router: ClientRouter) {
    RouterService.router = router;
  }

  public static getRouter(): ClientRouter {
    return this.router;
  }
}
