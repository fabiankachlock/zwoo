<template>
  <div class="w-full p-1 rounded bg-error-light-bg dark:bg-error-dark-bg border-2 border-error-light-border dark:border-error-dark-border">
    <pre class="text-error-light-text dark:text-error-dark-text text-xs italic error-value">{{ title }}</pre>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
defineProps<{
  title: string;
}>();
</script>

<style scoped>
.error-value {
  white-space: normal;
}
</style>
