<template>
  <button class="block align-baseline italic text-sm tc-main-secondary my-1 tc-main-dark hover:opacity-70 transition relative" @click="emit('click')">
    <slot></slot>
  </button>
</template>

<script setup lang="ts">
import { defineEmits } from 'vue';

const emit = defineEmits<{
  (event: 'click'): void;
}>();
</script>
