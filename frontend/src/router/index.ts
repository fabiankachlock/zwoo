import { RouteRecordRaw, createRouter, createWebHistory } from 'vue-router';
import Menu from '../views/Menu.vue';
import Home from '../views/Home.vue';
import Landing from '../views/Landing.vue';
import { useConfig } from '@/core/adapter/config';
import { GameRoute } from './game';
import { MenuRoutes } from './menu';

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

router.beforeEach((to, _from, next) => {
  if (to.meta['requiresAuth'] === true && !useConfig().isLoggedIn) {
    return next((to.meta['redirect'] as string) || '/');
  }
  next();
});

export default router;
