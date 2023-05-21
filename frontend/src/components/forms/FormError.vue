<template>
  <div class="m-2">
    <Error v-if="error.length > 0" :errors="error" />
  </div>
</template>

<script setup lang="ts">
import Error from '@/components/misc/Error.vue';

defineProps<{
  error: string[];
}>();
</script>
