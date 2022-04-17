import { RouteRecordRaw } from 'vue-router';
import Menu from '../views/Menu.vue';

export const ThemesRoute: RouteRecordRaw = {
  path: '/themes',
  component: Menu,
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
