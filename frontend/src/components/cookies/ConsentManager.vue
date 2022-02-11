<template>
  <div v-if="showPopup">
    <CookieDialog />
  </div>
</template>

<script setup lang="ts">
import { useCookies } from '@/core/adapter/cookies';
import { computed, defineAsyncComponent } from 'vue';

const cookies = useCookies();
const showPopup = computed(() => cookies.popupOpen);
const CookieDialog = defineAsyncComponent(() => import(/* webpackChunkName: "cookies" */ './CookieDialog.vue'));
</script>
