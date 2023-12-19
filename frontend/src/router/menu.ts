import { RouteRecordRaw } from 'vue-router';

export const MenuRoutes: Array<RouteRecordRaw> = [
  {
    path: 'imprint',
    component: () => import('../views/Imprint.vue'),
    alias: ['impressum']
  },
  {
    path: 'privacy',
    component: () => import('../views/Privacy.vue'),
    alias: ['datenschutz']
  },
  {
    path: 'contact',
    component: () => import('../views/Contact.vue'),
    alias: ['kontakt'],
    meta: {
      excludeEnv: 'offline'
    }
  },
  {
    path: 'login',
    component: () => import('../views/Login.vue'),
    meta: {
      noAuth: true,
      includeEnv: 'online',
      envRedirect: '/login-local'
    }
  },
  {
    path: 'login-local',
    component: () => import('../views/LoginLocal.vue'),
    meta: {
      noAuth: true,
      envOnly: 'local',
      redirect: '/home'
    }
  },
  {
    path: 'logout',
    component: () => import('../views/Logout.vue'),
    meta: {
      requiresAuth: true,
      excludeEnv: 'offline'
    }
  },
  {
    path: 'create-account',
    component: () => import('../views/CreateAccount.vue'),
    meta: {
      noAuth: true,
      includeEnv: 'online'
    }
  },
  {
    path: 'request-password-reset',
    component: () => import('../views/RequestPasswordReset.vue'),
    meta: {
      noAuth: true,
      includeEnv: 'online'
    }
  },
  {
    path: 'reset-password',
    component: () => import('../views/ResetPassword.vue'),
    meta: {
      noAuth: true,
      includeEnv: 'online'
    }
  },
  {
    path: 'verify-account',
    component: () => import('../views/VerifyAccount.vue'),
    meta: {
      includeEnv: 'online'
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
      excludeEnv: 'offline'
    }
  },
  {
    path: 'join/:id',
    component: () => import('../views/JoinGame.vue'),
    meta: {
      requiresAuth: true,
      redirect: true,
      excludeEnv: 'offline'
    }
  },
  {
    path: 'available-games',
    component: () => import('../views/GamesList.vue'),
    alias: ['available', 'list', 'games'],
    meta: {
      requiresAuth: true,
      excludeEnv: 'offline'
    }
  },
  {
    path: 'missing-cookies',
    component: () => import('../views/MissingCookies.vue'),
    meta: {
      excludeEnv: 'offline'
    }
  },
  {
    path: 'leaderboard',
    component: () => import('../views/Leaderboard.vue'),
    meta: {
      excludeEnv: 'offline'
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
      excludeEnv: 'offline'
    }
  },
  {
    path: 'offline',
    component: () => import('../views/Offline.vue'),
    meta: {
      envOnly: 'offline',
      onlineRedirect: '/home'
    }
  }
];
