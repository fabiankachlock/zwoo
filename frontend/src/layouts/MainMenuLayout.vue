<template>
  <div id="_layout__main-menu" class="bg-main">
    <div class="header fixed top-0 left-0 right-0 bg-darkest h-10 bc-primary border-b-1 shadow-sm z-30">
      <header class="h-full">
        <div class="h-full mx-3 my-0 relative flex justify-between items-center flex-row">
          <router-link to="/home" class="cursor-pointer flex flex-row items-center justify-start">
            <img src="/img/logo/zwoo_logo_simple_none.svg" class="h-8 w-8 mr-2" />
            <h1 class="tc-primary text-4xl">zwoo</h1>
          </router-link>
          <NavBar />
        </div>
      </header>
    </div>
    <div class="floating-menu relative z-40">
      <FloatingMenu />
    </div>
    <div class="main-content">
      <div class="pt-2 pb-2 mt-10 mb-8 relative">
        <div class="w-full sticky z-10 bg-main top-10">
          <slot name="sticky-content"></slot>
        </div>
        <div class="z-0">
          <slot></slot>
        </div>
        <!-- workaround for ios hidden content on the end of the side -->
        <div class="h-14"></div>
      </div>
      <div class="footer fixed bottom-0 left-0 right-0 bg-darkest h-8 bc-primary border-t-1 z-30">
        <footer class="h-full">
          <div class="h-8 py-1 flex justify-between items-center mx-3">
            <router-link to="/">
              <Icon icon="mi:home" class="text-xl tc-main" />
            </router-link>
            <div class="flex py-1">
              <span class="footer-item">© IGD20 {{ year }}</span>
              <router-link to="/imprint" class="footer-item">Imprint</router-link>
              <router-link to="/privacy" class="footer-item">Privacy</router-link>
            </div>
            <button class="flex items-center">
              <Icon icon="material-symbols:help-outline-rounded" class="text-xl tc-main mr-1" />
              <span class="tc-main"> Help </span>
            </button>
          </div>
        </footer>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import FloatingMenu from '@/components/menu/FloatingMenu.vue';
import NavBar from '@/components/menu/NavBar.vue';
import { Icon } from '@/components/misc/Icon';

const year = new Date().getFullYear();
</script>

<style lang="css" scoped>
/* notch phone things */
.header {
  padding: env(safe-area-inset-top) env(safe-area-inset-right) 0 env(safe-area-inset-left);
}

.footer-item:not(:last-child) {
  @apply border-r bc-lightest px-3;
}

a.footer-item {
  text-decoration: underline;
}

.footer-item,
.footer-item:last-child {
  @apply px-3 tc-main-secondary;
}

.main-content {
  padding-top: env(safe-area-inset-top);
  padding-right: env(safe-area-inset-right);
  padding-bottom: env(safe-area-inset-bottom);
  padding-left: env(safe-area-inset-left);
}
</style>
