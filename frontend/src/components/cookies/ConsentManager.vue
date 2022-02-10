<template>
  <CookieDialog />
</template>

<script setup lang="ts">
import { defineAsyncComponent } from 'vue';

const CookieDialog = defineAsyncComponent(() => import(/* webpackChunkName: "cookies" */ './CookieDialog.vue'));
</script>
