<template>
  <div></div>
</template>

<script setup lang="ts">
import { useConfig } from '@/core/adapter/config';

useConfig().logout();
</script>
