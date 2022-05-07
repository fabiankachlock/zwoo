<template>
  <div class="relative w-full h-full max-w-full max-h-full">
    <img
      v-for="(layer, i) in layers"
      :src="layer"
      :key="i"
      :alt="`${alt} - layer ${i}`"
      class="absolute"
      :class="imageClass"
      :style="`z-index: ${i + 1};${imageStyle}`"
    />
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';

defineProps<{
  layers: string[];
  alt: string;
  imageStyle?: string;
  imageClass?: string;
}>();
</script>
