import { RouteRecordRaw } from 'vue-router';

export const ThemesRoute: RouteRecordRaw = {
  path: '/themes',
  component: () => import(/* webpackChunkName: "themes" */ '../views/themes/_Layout.vue'),
  children: [
    {
      path: 'gallery',
      component: () => import(/* webpackChunkName: "themes" */ '../views/themes/Gallery.vue'),
      meta: {
        requiresAuth: false
      }
    }
  ]
};
