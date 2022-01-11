<template>
  <div class="fixed top-0 left-0 right-0 bg-darkest h-10 bc-primary border-b-1 shadow-sm z-30">
    <header class="h-full">
      <div class="h-full mx-4 my-0 relative flex justify-between items-center flex-row">
        <router-link to="/home" class="cursor-pointer flex flex-row items-center justify-start">
          <img src="/img/logo/zwoo_logo_simple_none.svg" class="max-h-8 mr-4" />
          <h1 class="tc-primary text-4xl">zwoo</h1>
        </router-link>
        <NavBar />
      </div>
    </header>
  </div>
  <div class="relative z-50">
    <FloatingMenu />
  </div>
  <div class="w-screen h-screen overflow-y-scroll relative">
    <div class="pt-10 w-full">
      <div class="relative w-full">
        <router-view></router-view>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import NavBar from '@/components/menu/NavBar.vue';
import FloatingMenu from '@/components/menu/FloatingMenu.vue';
</script>
