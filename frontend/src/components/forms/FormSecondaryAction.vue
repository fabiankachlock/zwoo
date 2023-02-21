<template>
  <div
    class="block text-xs tc-main-secondary mt-1 mb-2 tc-main-dark transition cursor-pointer hover:bg-light w-[80%] rounded-full py-1"
    @click="emit('click')"
  >
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { defineEmits } from 'vue';

const emit = defineEmits<{
  (event: 'click'): void;
}>();
</script>
