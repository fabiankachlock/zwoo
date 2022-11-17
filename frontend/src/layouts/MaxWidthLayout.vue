<template>
  <div id="_layout__max-width" :class="`${classMap[size ?? 'normal']} mx-auto relative py-0 px-6 ${classes ?? ''}`">
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
// eslint-disable-next-line no-undef
defineProps<{
  size?: 'small' | 'normal' | 'huge';
  classes?: string;
}>();

const classMap = {
  small: 'max-w-lg',
  normal: 'max-w-4xl',
  huge: 'max-w-7xl'
};
</script>
