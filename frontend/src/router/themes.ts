import { RouteRecordRaw } from 'vue-router';

export const ThemesRoute: RouteRecordRaw = {
  path: '/themes',
  component: () => import('../views/themes/_Layout.vue'),
  children: [
    {
      path: 'gallery',
      component: () => import('../views/themes/Gallery.vue'),
      meta: {
        requiresAuth: false
      }
    }
  ]
};
