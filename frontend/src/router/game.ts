import { RouteRecordRaw } from 'vue-router';

export const GameRoute: RouteRecordRaw = {
  path: '/game',
  component: () => import(/* webpackChunkName: "in-game" */ '../views/game/Index.vue'),
  children: [
    {
      path: 'play',
      component: () => import(/* webpackChunkName: "in-game" */ '../views/game/Play.vue'),
      meta: {
        requiresAuth: true,
        redirect: true
      }
    },
    {
      path: 'wait',
      component: () => import(/* webpackChunkName: "in-game" */ '../views/game/WaitingRoom.vue'),
      meta: {
        requiresAuth: true
      }
    },
    {
      path: 'summary',
      component: () => import(/* webpackChunkName: "in-game" */ '../views/game/Summary.vue'),
      meta: {
        requiresAuth: true
      }
    }
  ]
};
