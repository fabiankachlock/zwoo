<template>
  <div
    class="block text-xs text-text-secondary mt-1 mb-2 text-text-dark transition cursor-pointer hover:bg-bg-hover w-[80%] rounded-full py-1"
    @click="emit('click')"
  >
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
const emit = defineEmits<{
  (event: 'click'): void;
}>();
</script>
