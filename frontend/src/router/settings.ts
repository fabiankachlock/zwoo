import { RouteRecordRaw } from 'vue-router';

import { AppConfig } from '@/config';

export const SettingsRoutes: Array<RouteRecordRaw> = [
  {
    path: '/settings',
    component: () => import('../views/settings/_Layout.vue'),
    redirect: '/settings/general',
    children: [
      {
        path: 'general',
        component: () => import('../views/settings/General.vue')
      },
      {
        path: 'account',
        component: () => import('../views/settings/Account.vue'),
        meta: {
          excludeEnv: 'offline',
          envRedirect: '/settings'
        }
      },
      {
        path: 'game',
        component: () => import('../views/settings/Game.vue')
      },
      {
        path: 'developers',
        component: () => import('../views/settings/Developers.vue')
      },
      {
        path: 'about',
        component: () => import('../views/settings/About.vue')
      },
      ...(AppConfig.IsTauri
        ? [
            {
              path: 'server',
              component: () => import('../views/settings/LocalServer.vue')
            }
          ]
        : [])
    ]
  },
  {
    path: '/settings/:pathMatch(.*)*',
    redirect: '/settings'
  }
];
