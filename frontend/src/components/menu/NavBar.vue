<template>
  <div class="h-full flex items-center relative">
    <div @click="toggleMenu" class="sm:hidden text-3xl tc-main-secondary cursor-pointer">
      <Icon :icon="icon" class="tc-main-secondary" />
    </div>
    <div :class="{ hidden: !isMenuOpen, block: isMenuOpen }" class="fixed inset-0" @click="closeMenu"></div>
    <div
      :class="{ hidden: !isMenuOpen, block: isMenuOpen }"
      class="bg-darkest bc-invert-lightest fixed text-base z-10 py-2 list-none text-left rounded mt-1 right-4 top-7 border-1 shadow-sm sm:hidden"
      style="min-width: 10rem"
      @click="closeMenu"
    >
      <NavBarLinks />
    </div>
    <div class="hidden sm:flex flex-row list-none">
      <NavBarLinks />
    </div>
  </div>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { computed, ref } from 'vue';

import NavBarLinks from './NavBarLinks.vue';

const isMenuOpen = ref(false);
const icon = computed(() => (isMenuOpen.value ? 'gg:close' : 'gg:menu-grid-o'));

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value;
};

const closeMenu = () => {
  isMenuOpen.value = false;
};
</script>
