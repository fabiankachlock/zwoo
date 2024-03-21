import { Router } from 'vue-router';

export class RouterService {
  private static router: Router;

  public static registerRouter(router: Router) {
    RouterService.router = router;
  }

  public static getRouter(): Router {
    return this.router;
  }
}
