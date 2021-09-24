<template>
  <div class="bg-darkest h-10 bc-primary border-b-1 shadow-sm">
    <header class="h-full">
      <div class="h-full mx-4 my-0 relative flex justify-between items-center flex-row">
        <router-link to="/home" class="cursor-pointer">
          <h1 class="tc-primary text-4xl">zwoo</h1>
        </router-link>
        <NavBar />
      </div>
    </header>
  </div>
  <router-view></router-view>
</template>

<script setup lang="ts">
import NavBar from '@/components/menu/NavBar.vue';
</script>
