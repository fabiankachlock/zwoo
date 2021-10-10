<template>
  <div class="fixed top-0 left-0 right-0 bg-darkest h-10 bc-primary border-b-1 shadow-sm z-30">
    <header class="h-full">
      <div class="h-full mx-4 my-0 relative flex justify-between items-center flex-row">
        <router-link to="/home" class="cursor-pointer">
          <h1 class="tc-primary text-4xl">zwoo</h1>
        </router-link>
        <NavBar />
      </div>
    </header>
  </div>
  <div class="relative z-50">
    <FloatingMenu />
  </div>
  <div class="h-screen w-screen relative flex flex-col overflow-hidden z-10">
    <div class="pt-10 h-full w-full">
      <div class="relative w-full h-full">
        <div class="absolute top-0 bottom-0 left-0 right-0 overflow-y-auto">
          <router-view></router-view>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import NavBar from '@/components/menu/NavBar.vue';
import FloatingMenu from '@/components/menu/FloatingMenu.vue';
</script>
