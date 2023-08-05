<template>
  <button
    class="py-2 px-6 w-full rounded focus:outline-none transition border-2 border-transparent relative"
    :class="{
      'tc-main-secondary bg-main cursor-not-allowed': disabled,
      'tc-primary bg-dark hover:bg-darkest hover:border-primary': !disabled
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
