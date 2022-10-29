<template>
  <button
    class="block text-xs tc-main-secondary mt-1 mb-2 tc-main-dark transition cursor-pointer hover:bg-light w-[80%] rounded-full py-1"
    :href="link"
    @click="emit('click')"
  >
    <slot></slot>
  </button>
</template>

<script setup lang="ts">
import { defineEmits, defineProps } from 'vue';

defineProps<{
  link?: string;
}>();

const emit = defineEmits<{
  (event: 'click'): void;
}>();
</script>
