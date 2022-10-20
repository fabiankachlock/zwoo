<template>
  <div v-if="showPopup">
    <CookieDialog />
  </div>
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent } from 'vue';

import { useCookies } from '@/core/adapter/cookies';

const cookies = useCookies();
const showPopup = computed(() => cookies.popupOpen);
const CookieDialog = defineAsyncComponent(() => import(/* webpackChunkName: "cookies" */ './CookieDialog.vue'));
</script>
