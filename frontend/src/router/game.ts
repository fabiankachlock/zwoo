import { RouteRecordRaw } from 'vue-router';

export const GameRoutes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'menu',
    component: () => import(/* webpackChunkName: "in-game" */ '../views/game/Index.vue'),
    redirect: '/home',
    children: [
      {
        path: '/play',
        component: () => import(/* webpackChunkName: "in-game" */ '../views/game/Play.vue'),
        meta: {
          requiresAuth: true
        }
      },
      {
        path: '/wait',
        component: () => import(/* webpackChunkName: "in-game" */ '../views/game/WaitingRoom.vue'),
        meta: {
          requiresAuth: true
        }
      },
      {
        path: '/summary',
        component: () => import(/* webpackChunkName: "in-game" */ '../views/game/Summary.vue'),
        meta: {
          requiresAuth: true
        }
      }
    ]
  }
];
