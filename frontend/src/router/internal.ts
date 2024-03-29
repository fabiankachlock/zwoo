import { RouteRecordRaw } from 'vue-router';

import Index from '../views/internal/_Layout.vue';

export const InternalRoute: RouteRecordRaw = {
  path: '/_internal_',
  component: Index,
  children: [
    {
      path: 'pop-out-chat',
      component: () => import('../views/internal/PopOutChat.vue'),
      meta: {
        requiresAuth: false
      }
    }
  ]
};
