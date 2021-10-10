import { RouteRecordRaw, createRouter, createWebHistory } from 'vue-router';
import CreateAccount from '../views/CreateAccount.vue';
import Login from '../views/Login.vue';
import Logout from '../views/Logout.vue';
import Menu from '../views/Menu.vue';
import Home from '../views/Home.vue';
import Landing from '../views/Landing.vue';
import Settings from '../views/Settings.vue';
import Tutorial from '../views/Tutorial.vue';
import CreateGame from '../views/CreateGame.vue';
import JoinGame from '../views/JoinGame.vue';
import { useConfig } from '@/core/adapter/config';
import { GameRoutes } from './game';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'menu',
    component: Menu,
    redirect: '/home',
    children: [
      {
        path: '/settings',
        component: Settings
      },
      {
        path: '/login',
        component: Login
      },
      {
        path: '/logout',
        component: Logout
      },
      {
        path: '/create-account',
        component: CreateAccount
      },
      {
        path: '/tutorial',
        component: Tutorial
      },
      {
        path: '/create-game',
        component: CreateGame,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: '/join/:id',
        component: JoinGame
      },
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
      }
    ]
  },
  ...GameRoutes
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
