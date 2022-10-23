import { RouteRecordRaw } from 'vue-router';
import Index from '../views/internal/Index.vue';

export const InternalRoute: RouteRecordRaw = {
  path: '/_internal_',
  component: Index,
  children: [
    {
      path: 'pop-out-chat',
      component: () => import(/* webpackChunkName: "internal" */ '../views/internal/PopOutChat.vue'),
      meta: {
        requiresAuth: false
      }
    }
  ]
};
