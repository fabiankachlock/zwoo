<template>
  <router-view />
</template>

<script setup lang="ts">
import { useConfig } from './stores/config';
useConfig().configure(); // load stored config from localStorage
</script>
