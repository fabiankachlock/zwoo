import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import Landing from "../pages/Landing.vue";
import Login from "../pages/Login.vue";
import CreateAccount from "../pages/CreateAccount.vue";

export const Routes: RouteRecordRaw[] = [
  {
    path: "/",
    component: Landing,
    meta: {
      requiresAuth: true,
    },
  },
  {
    path: "/login",
    component: Login,
  },
  {
    path: "/create-account",
    component: CreateAccount,
  },
];

export const Router = createRouter({
  history: createWebHistory(),
  routes: Routes,
});

Router.beforeEach(async (to, from, next) => {
  if (to.meta.requiresAuth) {
    next("/login");
  } else {
    next();
  }
});
