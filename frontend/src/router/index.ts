import { RouteRecordRaw, createRouter, createWebHistory } from 'vue-router';
import Menu from '../views/Menu.vue';
import Home from '../views/Home.vue';
import Landing from '../views/Landing.vue';
import { GameRoute } from './game';
import { MenuRoutes } from './menu';
import { useAuth } from '@/core/adapter/auth';

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
  GameRoute
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

router.beforeEach(async (to, _from, next) => {
  const auth = useAuth();

  if (to.meta['requiresAuth'] === true) {
    if (!auth.isInitialized) {
      await auth.askStatus();
    }

    if (!auth.isLoggedIn) {
      const redirect = to.meta['redirect'] as string | boolean | undefined;

      if (redirect === true) {
        return next('/login?redirect=' + to.fullPath);
      }

      return next(redirect || '/');
    }
  }
  next();
});

export default router;
