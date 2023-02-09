<template>
  <MaxWidthLayout size="normal" class="imprint tc-main-secondary">
    <h1 class="tc-main">Datenschutz</h1>
  </MaxWidthLayout>
</template>

<script setup lang="ts">
import MaxWidthLayout from '@/layouts/MaxWidthLayout.vue';
</script>

<style scoped>
.imprint p {
  @apply my-2 text-lg;
}

.imprint h1 {
  @apply mt-4 mb-2 text-4xl;
}

.imprint h2 {
  @apply mt-4 mb-2 text-2xl;
}

.imprint h3 {
  @apply mt-4 mb-2 text-xl;
}
</style>
