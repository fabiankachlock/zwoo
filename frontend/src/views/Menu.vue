<template>
  <div class="__menu bg-main">
    <div class="menu fixed top-0 left-0 right-0 bg-darkest h-10 bc-primary border-b-1 shadow-sm z-30">
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
    <div class="relative z-40">
      <FloatingMenu />
    </div>
    <div class="menu-main w-screen pt-10">
      <div class="device-padding w-full h-full relative">
        <div class="absolute inset-0 overflow-y-auto">
          <router-view></router-view>
          <div class="h-10"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import NavBar from '@/components/menu/NavBar.vue';
import FloatingMenu from '@/components/menu/FloatingMenu.vue';
</script>

<style scoped>
.__menu {
  @apply fixed top-0 left-0 right-0 bottom-0 overflow-hidden;
  height: 100vh;
  max-height: calc(100vh - calc(100vh - 100%));
}
.menu-main {
  height: 100vh;
  max-height: calc(100vh - calc(100vh - 100% - 2.5rem));
}
.menu {
  padding: env(safe-area-inset-top) env(safe-area-inset-right) 0 env(safe-area-inset-left);
}
</style>
