import { RouteRecordRaw } from 'vue-router';

export const SettingsRoutes: Array<RouteRecordRaw> = [
  {
    path: '/settings',
    component: () => import('../views/settings/Settings.vue'),
    redirect: '/settings/general',
    children: [
      {
        path: 'general',
        component: () => import('../components/settings/sections/SettingsSectionGeneral.vue')
      },
      {
        path: 'account',
        component: () => import('../components/settings/sections/SettingsSectionAccount.vue')
      },
      {
        path: 'game',
        component: () => import('../components/settings/sections/SettingsSectionGame.vue')
      },
      {
        path: 'developers',
        component: () => import('../components/settings/sections/SettingsSectionDevelopers.vue')
      },
      {
        path: 'about',
        component: () => import('../components/settings/sections/SettingsSectionAbout.vue')
      }
    ]
  },
  {
    path: '/settings/:pathMatch(.*)*',
    redirect: '/settings'
  }
];
