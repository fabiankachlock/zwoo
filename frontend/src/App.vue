<template>
  <router-view />
</template>

<script setup lang="ts">
import { useConfig } from './core/adapter/config';
useConfig().configure(); // load stored config from localStorage
</script>
