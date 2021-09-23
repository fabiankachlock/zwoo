<template>
  <li class="px-2 py-1">
    <router-link v-if="!isExternal" :to="link" class="link tc-main-dark">
      {{ title }}
    </router-link>
    <a v-if="isExternal" :href="link" class="link tc-main-dark" target="_blank">
      {{ title }} <Icon icon="bytesize-external" class="tc-main ml-2" />
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
  @apply flex flex-row justify-start items-center transform ease-linear transition-transform md:hover:-translate-y-1 md:hover:translate-x-0 hover:translate-x-1;
}
</style>
