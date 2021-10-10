import { RouteRecordRaw } from 'vue-router';

export const MenuRoutes: Array<RouteRecordRaw> = [
  {
    path: '/settings',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Settings.vue')
  },
  {
    path: '/login',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Login.vue')
  },
  {
    path: '/logout',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Logout.vue')
  },
  {
    path: '/create-account',
    component: () => import(/* webpackChunkName: "menu" */ '../views/CreateAccount.vue')
  },
  {
    path: '/tutorial',
    component: () => import(/* webpackChunkName: "menu" */ '../views/Tutorial.vue')
  },
  {
    path: '/create-game',
    component: () => import(/* webpackChunkName: "menu" */ '../views/CreateGame.vue'),
    meta: {
      requiresAuth: true
    }
  },
  {
    path: '/join/:id',
    component: () => import(/* webpackChunkName: "menu" */ '../views/JoinGame.vue'),
    meta: {
      requiresAuth: true
    }
  }
];
