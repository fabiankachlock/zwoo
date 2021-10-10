<template>
  <router-view />
</template>

<script setup lang="ts">
import { useAuth } from './core/adapter/auth';
import { useConfig } from './core/adapter/config';

useConfig().configure(); // load stored config from localStorage
useAuth().configure(); // 'read' from may existing session
</script>
