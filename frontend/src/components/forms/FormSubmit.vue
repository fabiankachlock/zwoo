<template>
  <button
    class="py-2 px-6 w-full bg-alt rounded transition border-2 border-transparent relative outline-none"
    :class="{
      'text-text-secondary cursor-not-allowed pointer-events-none': disabled,
      'text-primary-text hover:bg-alt-hover focus:border-primary': !disabled
    }"
    type="button"
    :tabindex="disabled ? -1 : 0"
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
