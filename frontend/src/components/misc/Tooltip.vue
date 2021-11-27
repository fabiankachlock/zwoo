<template>
  <div v-tooltip="title">
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';

defineProps<{
  title: string;
}>();
</script>
