import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';

import { ShortcutManager } from '@/core/adapter/shortcuts/ShortcutManager';
import { AuthGuard } from '@/core/services/security/AuthGuard';
import { CookieGuard } from '@/core/services/security/CookieGuard';
import { InGameGuard } from '@/core/services/security/GameGuard';
import { ReCaptchaTermsRouteInterceptor } from '@/core/services/security/ReCaptchaTerms';
import { VersionGuard } from '@/core/services/security/VersionGuard';

import CatchAll from '../views/404.vue';
import Beta from '../views/Beta.vue';
import Home from '../views/Home.vue';
import Landing from '../views/Landing.vue';
import Menu from '../views/Menu.vue';
import Version from '../views/Version.vue';
import { DeveloperRoute } from './developer';
import { GameRoute } from './game';
import { InternalRoute } from './internal';
import { MenuRoutes } from './menu';
import { ThemesRoute } from './themes';
import { RouterInterceptor } from './types';

const routes: Array<RouteRecordRaw> = [
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
          redirect: '/landing'
        }
      },
      {
        path: '/landing',
        component: Landing
      },
      ...MenuRoutes
    ]
  },
  GameRoute,
  ThemesRoute,
  DeveloperRoute,
  InternalRoute,
  {
    path: '/invalid-version',
    component: Version
  },
  import.meta.env.VUE_APP_BETA === 'true'
    ? {
        path: '/beta/:code',
        component: Beta
      }
    : {
        path: '/beta/:code',
        redirect: '/'
      },
  {
    path: '/:pathMatch(.*)*',
    component: CatchAll
  }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
});

const BeforeEachSyncGuards: RouterInterceptor['beforeEach'][] = [
  new VersionGuard().beforeEach,
  new AuthGuard().beforeEach,
  new CookieGuard().beforeEach,
  new InGameGuard().beforeEach
];
const BeforeEachAsyncGuards: RouterInterceptor['beforeEachAsync'][] = [];

router.beforeEach(async (to, from, next) => {
  let called = false;
  for (const guard of BeforeEachSyncGuards) {
    if (guard) {
      const redirected = await guard(to, from, next);
      called ||= redirected;
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
  new ReCaptchaTermsRouteInterceptor().afterEachAsync,
  ShortcutManager.global.afterEachAsync
];
// (async () => ([
//   new (await import(/* webpackChunkName: "recaptcha" */ '../core/services/security/ReCaptchaTerms')).default().afterEachAsync
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
