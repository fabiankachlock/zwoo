<template>
  <div class="h-full flex items-center relative">
    <DynamicActions></DynamicActions>
    <div class="ml-2 text-3xl tc-main-secondary cursor-pointer" @click="toggleMenu">
      <Icon :icon="icon" class="tc-main-secondary" />
    </div>
    <div :class="{ hidden: !isMenuOpen, block: isMenuOpen }" class="fixed inset-0" @click="closeMenu"></div>
    <div
      :class="{ 'max-h-0': !isMenuOpen, 'open max-h-[500px]': isMenuOpen }"
      class="menu-container bg-surface border-1 bc-primary fixed z-10 right-0 transition duration-300 overflow-hidden shadow-sm"
      style="min-width: 14rem"
      @click="closeMenu"
    >
      <div>
        <MenuItems></MenuItems>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';

import { Icon } from '@/components/misc/Icon';

import DynamicActions from './DynamicActions.vue';
import MenuItems from './MenuItems.vue';

const isMenuOpen = ref(false);
const icon = computed(() => (isMenuOpen.value ? 'gg:close' : 'gg:menu-grid-o'));

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value;
};

const closeMenu = () => {
  isMenuOpen.value = false;
};
</script>

<style scoped>
.menu-container {
  top: calc(2.5rem + env(safe-area-inset-top) - 1px);
  border-right-width: 0px;
  border-top-width: 0px;
  border-radius: 0 0 0 0.5rem;
  transition-property: max-height;
  height: fit-content;
}

.menu-container.open {
  top: calc(2.5rem + env(safe-area-inset-top));
}
</style>
