<template>
  <a class="block align-baseline italic text-xs tc-main-secondary my-1 tc-main-dark hover:opacity-70 transition" :href="link" @click="emit('click')">
    <slot></slot>
  </a>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue';

defineProps<{
  link?: string;
}>();

const emit = defineEmits<{
  (event: 'click'): void;
}>();
</script>
