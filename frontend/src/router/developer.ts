import { RouteRecordRaw } from 'vue-router';

export const DeveloperRoute: RouteRecordRaw = {
  path: '/__dev__',
  component: () => import(/* webpackChunkName: "dev-settings" */ '../views/__dev__/Index.vue'),
  children: [
    {
      path: 'logging',
      component: () => import(/* webpackChunkName: "dev-settings" */ '../views/__dev__/Logging.vue')
    }
  ]
};
