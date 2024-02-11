<script setup lang="ts">
import { onMounted } from 'vue';

import { useRootApp } from '@/core/adapter/app';

const app = useRootApp();

onMounted(() => {
  console.log(app.isLoading);
});
</script>

<template>
  <div>
    <h1>EnvLock</h1>
  </div>
</template>
