<script setup lang="ts">
import { onMounted } from 'vue';

onMounted(() => {
  const urlParams = new URLSearchParams(window.location.search);
  const url = urlParams.get('t');
  window.location.href = url ?? '/';
});
</script>

<template>Redirecting</template>
