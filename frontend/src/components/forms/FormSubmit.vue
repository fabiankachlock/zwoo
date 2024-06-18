<template>
  <button
    class="py-2 px-6 w-full bg-alt rounded focus:outline-none transition border-2 border-transparent relative"
    :class="{
      'text-text-secondary cursor-not-allowed': disabled,
      'text-primary-text hover:bg-alt-hover hover:border-primary': !disabled
    }"
    type="button"
    @click="emit('click')"
  >
    <slot></slot>
  </button>
</template>

<script setup lang="ts">
defineProps<{
  disabled?: boolean;
}>();

const emit = defineEmits<{
  (event: 'click'): void;
}>();
</script>
