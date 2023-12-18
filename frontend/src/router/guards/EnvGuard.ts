import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';

import { keepRedirect } from '@/composables/useRedirect';
import { useRootApp } from '@/core/adapter/app';
import { RouterInterceptor } from '@/router/types';

export class EnvGuard implements RouterInterceptor {
  beforeEach = async (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext): Promise<boolean> => {
    const env = useRootApp().environment;
    const envOnly = to.meta['envOnly'];
    const include = to.meta['includeEnv'] ?? [];
    const exclude = to.meta['excludeEnv'] ?? [];
    const normalizedInclude = Array.isArray(include) ? include : [include];
    const normalizedExclude = Array.isArray(exclude) ? exclude : [exclude];
    debugger;
    if ((envOnly === env || normalizedInclude.includes(env) || (!envOnly && normalizedInclude.length === 0)) && !normalizedExclude.includes(env)) {
      // good to go
      return false;
    }

    if (to.meta['envRedirect']) {
      next(keepRedirect(to.fullPath, to.meta['envRedirect'] as string));
    } else {
      next('/');
    }
    return true;
  };
}
