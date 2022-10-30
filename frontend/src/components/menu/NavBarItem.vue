<template>
  <li>
    <router-link v-if="!isExternal" :to="link" class="link tc-main-dark">
      {{ title }}
    </router-link>
    <a v-if="isExternal" :href="link" class="link tc-main-dark" target="_blank">
      {{ title }} <Icon icon="bytesize:external" class="tc-main ml-2" />
    </a>
  </li>
</template>

<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { defineProps } from 'vue';

defineProps<{
  title: string;
  isExternal?: boolean;
  link: string;
}>();
</script>

<style scoped>
.link {
  @apply flex flex-row justify-start cursor-pointer items-center px-3 sm:px-0 ease-linear transition-transform sm:text-sm sm:hover:-translate-y-1 sm:hover:translate-x-0 hover:translate-x-1;
}
</style>
