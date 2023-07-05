import { RouteRecordRaw } from 'vue-router';

export const MenuRoutes: Array<RouteRecordRaw> = [
  {
    path: 'imprint',
    component: () => import('../views/Imprint.vue')
  },
  {
    path: 'privacy',
    component: () => import('../views/Privacy.vue')
  },
  {
    path: 'contact',
    component: () => import('../views/Contact.vue')
  },
  {
    path: 'login',
    component: () => import('../views/Login.vue'),
    meta: {
      noAuth: true,
      onlineOnly: true
    }
  },
  {
    path: 'logout',
    component: () => import('../views/Logout.vue'),
    meta: {
      requiresAuth: true,
      onlineOnly: true
    }
  },
  {
    path: 'create-account',
    component: () => import('../views/CreateAccount.vue'),
    meta: {
      noAuth: true,
      onlineOnly: true
    }
  },
  {
    path: 'request-password-reset',
    component: () => import('../views/RequestPasswordReset.vue'),
    meta: {
      noAuth: true,
      onlineOnly: true
    }
  },
  {
    path: 'reset-password',
    component: () => import('../views/ResetPassword.vue'),
    meta: {
      noAuth: true,
      onlineOnly: true
    }
  },
  {
    path: 'verify-account',
    component: () => import('../views/VerifyAccount.vue'),
    meta: {
      onlineOnly: true
    }
  },
  {
    path: 'tutorial',
    component: () => import('../views/Tutorial.vue')
  },
  {
    path: 'create-game',
    component: () => import('../views/CreateGame.vue'),
    meta: {
      requiresAuth: true,
      onlineOnly: true
    }
  },
  {
    path: 'join/:id',
    component: () => import('../views/JoinGame.vue'),
    meta: {
      requiresAuth: true,
      redirect: true,
      onlineOnly: true
    }
  },
  {
    path: 'available-games',
    component: () => import('../views/GamesList.vue'),
    alias: ['available', 'list', 'games'],
    meta: {
      requiresAuth: true,
      onlineOnly: true
    }
  },
  {
    path: 'missing-cookies',
    component: () => import('../views/MissingCookies.vue'),
    meta: {
      onlineOnly: true
    }
  },
  {
    path: 'leaderboard',
    component: () => import('../views/Leaderboard.vue'),
    meta: {
      onlineOnly: true
    }
  },
  {
    path: 'shortcut-info',
    component: () => import('../views/ShortcutInfo.vue')
  },
  {
    path: 'version-history',
    component: () => import('../views/VersionHistory.vue'),
    meta: {
      onlineOnly: true
    }
  },
  {
    path: 'offline',
    component: () => import('../views/Offline.vue'),
    meta: {
      offlineOnly: true,
      onlineRedirect: '/home'
    }
  }
];
