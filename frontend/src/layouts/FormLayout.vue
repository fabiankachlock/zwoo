<template>
  <MaxWidthLayout size="small" classes="mt-4">
    <FlatDialog>
      <slot></slot>
    </FlatDialog>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import FlatDialog from '@/components/misc/FlatDialog.vue';

import MaxWidthLayout from './MaxWidthLayout.vue';
</script>
