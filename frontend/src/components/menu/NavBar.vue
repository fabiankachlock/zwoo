<template>
  <div class="h-full flex items-center relative">
    <div @click="toggleMenu" class="md:hidden text-3xl tc-main-secondary cursor-pointer">
      <Icon :icon="icon" class="tc-main-secondary" />
    </div>
    <div :class="{ hidden: !isMenuOpen, block: isMenuOpen }" class="fixed inset-0" @click="closeMenu"></div>
    <div
      :class="{ hidden: !isMenuOpen, block: isMenuOpen }"
      class="bg-darkest bc-invert-lightest fixed text-base z-10 py-2 list-none text-left rounded mt-1 right-4 top-7 border-1 shadow-sm md:hidden"
      style="min-width: 10rem"
    >
      <NavBarLinks :is-logged-in="isLoggedIn" />
    </div>
    <div class="hidden md:flex flex-row list-none">
      <NavBarLinks :is-logged-in="isLoggedIn" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuth } from '@/core/adapter/auth';
import { Icon } from '@iconify/vue';
import { computed, ref } from 'vue';
import NavBarLinks from './NavBarLinks.vue';

const auth = useAuth();

const isLoggedIn = computed(() => auth.isLoggedIn);
const isMenuOpen = ref(false);
const icon = computed(() => (isMenuOpen.value ? 'gg:close' : 'gg:menu-grid-o'));

const toggleMenu = () => {
  console.log('toggle');
  isMenuOpen.value = !isMenuOpen.value;
};

const closeMenu = () => {
  console.log('close');
  isMenuOpen.value = false;
};
</script>
