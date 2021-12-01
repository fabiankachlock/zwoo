<template>
  <div class="block align-baseline italic text-sm tc-main-secondary my-1 tc-main-dark hover:opacity-70 transition relative" @click="emit('click')">
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { defineEmits } from 'vue';

const emit = defineEmits<{
  (event: 'click'): void;
}>();
</script>
