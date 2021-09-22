import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import CreateAccount from '../views/CreateAccount.vue';
import Login from '../views/Login.vue';
import Menu from '../views/Menu.vue';
import Home from '../views/Home.vue';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'menu',
    component: Menu,
    redirect: '/home',
    children: [
      {
        path: '/login',
        component: Login
      },
      {
        path: '/create-account',
        component: CreateAccount
      },
      {
        path: '/home',
        component: Home
      }
    ]
  },
  {
    path: '/play',
    name: 'In Game',
    component: () => import(/* webpackChunkName: "in-game" */ '../views/game/Play.vue')
  }
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

export default router;
