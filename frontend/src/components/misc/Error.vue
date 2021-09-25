<template>
  <div class="w-full p-2 rounded-lg bg-error-light-bg dark:bg-error-dark-bg border-2 border-error-light-border dark:border-error-dark-border">
    <p class="text-error-light-text dark:text-error-dark-text text-lg">
      {{ title }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
defineProps<{
  title: string;
}>();
</script>
