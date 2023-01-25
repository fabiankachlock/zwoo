import { RouteRecordRaw } from 'vue-router';

export const MenuRoutes: Array<RouteRecordRaw> = [
  {
    path: 'settings',
    component: () => import('../views/Settings.vue')
  },
  {
    path: 'imprint',
    component: () => import('../views/Imprint.vue')
  },
  {
    path: 'login',
    component: () => import('../views/Login.vue'),
    meta: {
      noAuth: true
    }
  },
  {
    path: 'logout',
    component: () => import('../views/Logout.vue')
  },
  {
    path: 'create-account',
    component: () => import('../views/CreateAccount.vue'),
    meta: {
      noAuth: true
    }
  },
  {
    path: 'request-password-reset',
    component: () => import('../views/RequestPasswordReset.vue'),
    meta: {
      noAuth: true
    }
  },
  {
    path: 'reset-password',
    component: () => import('../views/ResetPassword.vue'),
    meta: {
      noAuth: true
    }
  },
  {
    path: 'verify-account',
    component: () => import('../views/VerifyAccount.vue')
  },
  {
    path: 'tutorial',
    component: () => import('../views/Tutorial.vue')
  },
  {
    path: 'create-game',
    component: () => import('../views/CreateGame.vue'),
    meta: {
      requiresAuth: true
    }
  },
  {
    path: 'join/:id',
    component: () => import('../views/JoinGame.vue'),
    meta: {
      requiresAuth: true,
      redirect: true
    }
  },
  {
    path: 'available-games',
    component: () => import('../views/GamesList.vue'),
    alias: ['available', 'list', 'games'],
    meta: {
      requiresAuth: true
    }
  },
  {
    path: 'missing-cookies',
    component: () => import('../views/MissingCookies.vue')
  },
  {
    path: 'leaderboard',
    component: () => import('../views/Leaderboard.vue')
  },
  {
    path: 'shortcut-info',
    component: () => import('../views/ShortcutInfo.vue')
  },
  {
    path: 'version-history',
    component: () => import('../views/VersionHistory.vue')
  },
  {
    path: 'offline',
    component: () => import('../views/Offline.vue')
  }
];
