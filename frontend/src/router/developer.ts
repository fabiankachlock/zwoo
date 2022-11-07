import { RouteRecordRaw } from 'vue-router';

export const DeveloperRoute: RouteRecordRaw = {
  path: '/__dev__',
  component: () => import('../views/__dev__/_Layout.vue'),
  children: [
    {
      path: 'logging',
      component: () => import('../views/__dev__/Logging.vue')
    }
  ]
};
