<template>
  <div class="bg-darkest h-10 border-primary border-b-2 shadow-sm">
    <header class="h-full">
      <div class="h-full mx-4 my-0 relative flex justify-between items-center flex-row">
        <h1 class="text-primary text-4xl">zwoo</h1>
        <div class="h-full flex items-center">
          <div @click="toggleMenu" class="md:hidden text-3xl">
            <Icon :icon="icon" />
          </div>
        </div>
      </div>
    </header>
  </div>
  <router-view></router-view>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { ref, computed } from 'vue';

const isMenuOpen = ref(false);
const icon = computed(() => (isMenuOpen.value ? 'gg:close' : 'gg:menu-grid-o'));

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value;
};
</script>
