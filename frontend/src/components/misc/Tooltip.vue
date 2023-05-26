<template>
  <div v-tooltip="title">
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  title: string;
}>();
</script>
