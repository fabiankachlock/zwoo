<template>
  <div class="w-full flex flex-row justify-between items-center bg-dark px-1 py-3 my-4 rounded-lg">
    <div class="h-full flex-1 mx-2 tc-main-dark">
      {{ title }}
    </div>
    <div class="mx-2 h-full flex items-center justify-end">
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';

defineProps<{
  title: string;
}>();
</script>
