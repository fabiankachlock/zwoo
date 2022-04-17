<template>
  <button
    class="bg-dark tc-primary py-2 px-6 w-full rounded focus:outline-none focus:shadow-outline transition hover:bg-darkest border-2 border-transparent hover:border-primary relative"
    type="button"
    @click="emit('click')"
  >
    <slot></slot>
  </button>
</template>

<script setup lang="ts">
import { defineEmits } from 'vue';

const emit = defineEmits<{
  (event: 'click'): void;
}>();
</script>
