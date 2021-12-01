<template>
  <button
    class="
      bg-darkest
      tc-main-light
      font-bold
      py-2
      px-4
      rounded
      focus:outline-none focus:shadow-outline
      transform
      transition
      hover:scale-95 hover:bg-dark
      relative
    "
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
