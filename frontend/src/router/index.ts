import { RouteRecordRaw, createRouter, createWebHistory } from 'vue-router';
import Menu from '../views/Menu.vue';
import Home from '../views/Home.vue';
import Landing from '../views/Landing.vue';
import CatchAll from '../views/404.vue';
import Beta from '../views/Beta.vue';
import { GameRoute } from './game';
import { MenuRoutes } from './menu';
import { RouterInterceptor } from './types';
import { AuthGuard } from '@/core/services/security/AuthGuard';
import { ReCaptchaTermsRouteInterceptor } from '@/core/services/security/ReCaptchaTerms';
import { CookieGuard } from '@/core/services/security/CookieGuard';
import { InGameGuard } from '@/core/services/security/GameGuard';
import { DeveloperRoute } from './developer';
import { ThemesRoute } from './themes';
import { ShortcutManager } from '@/core/adapter/shortcuts/ShortcutManager';

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
  process.env.VUE_APP_BETA === 'true'
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
  history: createWebHistory(process.env.BASE_URL),
  routes
});

const BeforeEachSyncGuards: RouterInterceptor['beforeEach'][] = [
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
