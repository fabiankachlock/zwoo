import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';

import { AppConfig } from '@/config';
import { useRootApp } from '@/core/adapter/app';
import { ShortcutManager } from '@/core/adapter/shortcuts/ShortcutManager';
import { AuthGuard } from '@/router/guards/AuthGuard';
import { CookieGuard } from '@/router/guards/CookieGuard';
import { InGameGuard } from '@/router/guards/GameGuard';
// import { ReCaptchaTermsRouteInterceptor } from '@/router/guards/ReCaptchaTerms';
import { VersionGuard } from '@/router/guards/VersionGuard';
import Redirect from '@/views/Redirect.vue';

import Menu from '../views/_Layout.vue';
import CatchAll from '../views/404.vue';
import Beta from '../views/Beta.vue';
import Home from '../views/Home.vue';
import Landing from '../views/Landing.vue';
import Version from '../views/Version.vue';
import { DeveloperRoute } from './developer';
import { GameRoute } from './game';
import { EnvGuard } from './guards/EnvGuard';
import { InternalRoute } from './internal';
import { MenuRoutes } from './menu';
import { SettingsRoutes } from './settings';
import { ThemesRoute } from './themes';
import { RouterInterceptor } from './types';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/:pathMatch(.*)*',
    component: CatchAll
  },
  {
    path: '/',
    component: Menu,
    redirect: '/home',
    children: [
      {
        path: '/home',
        component: Home,
        meta: {
          requiresAuth: true,
          redirect: '/landing',
          excludeEnv: 'offline',
          envRedirect: '/offline'
        }
      },
      {
        path: '/landing',
        component: Landing,
        meta: {
          excludeEnv: ['offline', 'local'],
          envRedirect: {
            local: '/login-local',
            offline: '/offline'
          }
        }
      },
      ...MenuRoutes,
      ...SettingsRoutes
    ]
  },
  GameRoute,
  ThemesRoute,
  DeveloperRoute,
  InternalRoute,
  {
    path: '/redirect',
    component: Redirect
  },
  {
    path: '/invalid-version',
    component: Version,
    meta: {
      excludeEnv: 'offline'
    }
  },
  AppConfig.IsBeta
    ? {
        path: '/beta/:code',
        component: Beta,
        meta: {
          excludeEnv: 'offline'
        }
      }
    : {
        path: '/beta/:code',
        redirect: '/'
      }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior() {
    // always scroll to top
    return { top: 0 };
  }
});

const BeforeEachSyncGuards: RouterInterceptor['beforeEach'][] = [
  new EnvGuard().beforeEach,
  new VersionGuard().beforeEach,
  new AuthGuard().beforeEach,
  new CookieGuard().beforeEach,
  new InGameGuard().beforeEach
];
const BeforeEachAsyncGuards: RouterInterceptor['beforeEachAsync'][] = [];

router.beforeEach(async (to, from, next) => {
  if (to.path === '/redirect') {
    next();
    return;
  }

  // wait until everything is configured (to enable save env based redirects)
  const app = useRootApp();
  await app.isConfigured.promise;

  let called = false;
  for (const guard of BeforeEachSyncGuards) {
    if (guard) {
      const redirected = await guard(to, from, next);
      called ||= redirected;
      if (called) break;
    }
  }

  if (!called) {
    next();
  }

  Promise.all([
    ...BeforeEachAsyncGuards.map(guard => {
      if (guard) {
        guard(to, from);
      }
    })
  ]);
});

const AfterEachSyncGuards: RouterInterceptor['afterEach'][] = [];
const AfterEachAsyncGuards: RouterInterceptor['afterEachAsync'][] = [
  //new ReCaptchaTermsRouteInterceptor().afterEachAsync,
  ShortcutManager.global.afterEachAsync
];
// (async () => ([
//   new (await import( '../core/services/guards/ReCaptchaTerms')).default().afterEachAsync
// ]));

router.afterEach(async (to, from, failure) => {
  for (const guard of AfterEachSyncGuards) {
    if (guard) {
      await guard(from, to, failure);
    }
  }

  Promise.all([
    ...AfterEachAsyncGuards.map(guard => {
      if (guard) {
        guard(from, to, failure);
      }
    })
  ]);
});

export default router;
