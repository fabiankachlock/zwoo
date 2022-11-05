import { RouteRecordRaw } from 'vue-router';

export const GameRoute: RouteRecordRaw = {
  path: '/game',
  component: () => import('../views/game/_Layout.vue'),
  children: [
    {
      path: 'play',
      component: () => import('../views/game/Play.vue'),
      meta: {
        requiresAuth: true
      }
    },
    {
      path: 'wait',
      component: () => import('../views/game/WaitingRoom.vue'),
      meta: {
        requiresAuth: true
      }
    },
    {
      path: 'summary',
      component: () => import('../views/game/Summary.vue'),
      meta: {
        requiresAuth: true
      }
    }
  ]
};
