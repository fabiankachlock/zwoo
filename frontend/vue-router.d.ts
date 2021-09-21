import { ComponentCustomProperties } from "vue";
import { RouteLocationNormalizedLoaded, Router } from "vue-router";

declare module "@vue/runtime-core" {
  // provide typings for `this.$route` and `this.$router`
  interface ComponentCustomProperties {
    $route: RouteLocationNormalizedLoaded;
    $router: Router;
  }
}
