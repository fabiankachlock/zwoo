<template>
  <div
    class="
      w-full
      flex flex-row
      justify-between
      items-center
      bg-dark
      px-1
      py-3
      my-3
      rounded-lg
      border border-transparent
      hover:bc-primary hover:bg-darkest
    "
  >
    <div class="h-full flex-1 mx-2 tc-main-light">
      <span>{{ title }}</span>
      <span v-if="status" class="text-sm mx-1 tc-main-secondary">({{ status }})</span>
    </div>
    <div class="mx-2 h-full flex items-center justify-end">
      <Tooltip v-if="tooltip" :title="tooltip">
        <slot></slot>
      </Tooltip>
      <slot v-else></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
import Tooltip from '@/components/misc/Tooltip.vue';

defineProps<{
  title: string;
  status?: string;
  tooltip?: string;
}>();
</script>
