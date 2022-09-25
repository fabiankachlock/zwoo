import { RouteRecordRaw } from 'vue-router';

export const MenuRoutes: Array<RouteRecordRaw> = [
  {
    path: 'settings',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Settings.vue')
  },
  {
    path: 'imprint',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Imprint.vue')
  },
  {
    path: 'login',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Login.vue')
  },
  {
    path: 'logout',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Logout.vue')
  },
  {
    path: 'create-account',
    component: () => import(/* webpackChunkName: "menu" */ '../views/CreateAccount.vue')
  },
  {
    path: 'request-password-reset',
    component: () => import(/* webpackChunkName: "menu" */ '../views/RequestPasswordReset.vue')
  },
  {
    path: 'verify-account',
    component: () => import(/* webpackChunkName: "menu" */ '../views/VerifyAccount.vue')
  },
  {
    path: 'tutorial',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Tutorial.vue')
  },
  {
    path: 'create-game',
    component: () => import(/* webpackChunkName: "menu" */ '../views/CreateGame.vue'),
    meta: {
      requiresAuth: true
    }
  },
  {
    path: 'join/:id',
    component: () => import(/* webpackChunkName: "menu" */ '../views/JoinGame.vue'),
    meta: {
      requiresAuth: true,
      redirect: true
    }
  },
  {
    path: 'available-games',
    component: () => import(/* webpackChunkName: "menu" */ '../views/GamesList.vue'),
    alias: ['available', 'list', 'games'],
    meta: {
      requiresAuth: true
    }
  },
  {
    path: 'missing-cookies',
    component: () => import(/* webpackChunkName: "menu" */ '../views/MissingCookies.vue')
  },
  {
    path: 'leaderboard',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Leaderboard.vue')
  },
  {
    path: 'shortcut-info',
    component: () => import(/* webpackChunkName: "menu" */ '../views/ShortcutInfo.vue')
  }
];
