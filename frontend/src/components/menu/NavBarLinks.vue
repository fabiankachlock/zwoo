<template>
  <div v-if="!isLoggedIn" class="md:flex flex-row">
    <NavBarItem title="Login" link="/login" />
    <NavBarItem title="Create Account" link="/create-account" />
    <div class="divider bc-invert-main"></div>
  </div>
  <NavBarItem title="New Idea?" link="https://github.com/fabiankachlock/zwoo/discussions/categories/ideas" is-external />
  <NavBarItem title="Github" link="https://github.com/fabiankachlock/zwoo" is-external />
  <div v-if="isLoggedIn" class="md:flex flex-row">
    <div class="divider bc-invert-main"></div>
    <NavBarItem title="Logout" link="/logout" />
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
import NavBarItem from './NavBarItem.vue';

defineProps<{
  isLoggedIn?: boolean;
}>();
</script>

<style scoped>
.divider {
  @apply h-0 my-2 border border-solid border-t-0 opacity-50 md:hidden;
}
</style>
