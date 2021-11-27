import { NavigationFailure, NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

export interface RouterInterceptor {
  beforeEach?: (to: RouteLocationNormalized, from: RouteLocationNormalized, next: NavigationGuardNext) => Promise<boolean>;
  beforeEachAsync?: (to: RouteLocationNormalized, from: RouteLocationNormalized) => void;
  afterEach?: (from: RouteLocationNormalized, current: RouteLocationNormalized, failure: void | NavigationFailure | undefined) => void;
  afterEachAsync?: (from: RouteLocationNormalized, current: RouteLocationNormalized, failure: void | NavigationFailure | undefined) => void;
}

