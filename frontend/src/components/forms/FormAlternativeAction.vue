<template>
  <div class="block text-sm my-1 text-text relative bg-alt hover:bg-alt-hover w-full px-2 py-1 rounded mt-4" @click="emit('click')">
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
const emit = defineEmits<{
  (event: 'click'): void;
}>();
</script>
