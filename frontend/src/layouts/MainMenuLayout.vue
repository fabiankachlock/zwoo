<template>
  <div id="_layout__main-menu" class="bg-bg">
    <div class="header fixed top-0 left-0 right-0 bg-surface h-10 border-primary border-b shadow-sm z-30">
      <header class="h-full">
        <div class="h-full mx-3 my-0 relative flex justify-between items-center flex-row">
          <router-link to="/home" class="cursor-pointer flex flex-row items-center justify-start">
            <div class="logo h-8 w-8 mr-2" v-html="Logo"></div>
            <h1 class="text-primary-text text-3xl mb-[0.25rem]">ZWOO</h1>
          </router-link>
          <NavBar />
        </div>
      </header>
    </div>
    <div class="floating-menu relative z-40">
      <FloatingMenu />
    </div>
    <div class="main-content">
      <div class="pt-2 pb-2 margin-layout relative">
        <div class="w-full sticky z-10 bg-bg top-10">
          <slot name="sticky-content"></slot>
        </div>
        <div class="z-0">
          <slot></slot>
        </div>
        <!-- workaround for ios hidden content on the end of the side -->
        <div class="h-14"></div>
      </div>
      <div class="footer fixed bottom-0 left-0 right-0 bg-surface border-primary border-t z-30">
        <Footer />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import Logo from '@/assets/zwoo_logo_simple_none_auto.svg?raw';
import FloatingMenu from '@/components/menu/FloatingMenu.vue';
import Footer from '@/components/menu/Footer.vue';
import NavBar from '@/components/menu/NavBar.vue';
</script>

<style lang="css" scoped>
/* notch phone things */
.header {
  padding: env(safe-area-inset-top) env(safe-area-inset-right) 0 env(safe-area-inset-left);
  height: calc(2.5rem + env(safe-area-inset-top));
}

.footer {
  padding: 0 env(safe-area-inset-right) env(safe-area-inset-bottom) env(safe-area-inset-left);
  height: calc(2rem + env(safe-area-inset-bottom));
}

.margin-layout {
  margin-top: calc(2.5rem + env(safe-area-inset-top));
  margin-bottom: calc(2rem + env(safe-area-inset-bottom));
}

.main-content {
  padding-top: env(safe-area-inset-top);
  padding-right: env(safe-area-inset-right);
  padding-bottom: env(safe-area-inset-bottom);
  padding-left: env(safe-area-inset-left);
}

.logo :deep(svg) {
  width: 100%;
  height: auto;
}
</style>
