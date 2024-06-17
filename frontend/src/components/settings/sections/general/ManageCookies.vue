<template>
  <button
    class="flex justify-center items-center bg-bg border-2 border-transparent px-2 rounded transition hover:bg-bg cursor-pointer select-none"
    @click="manageCookies"
  >
    <!-- <p class="text-text text-center">{{ t('cookies.manage') }}</p> -->
    <p class="text-text text-center">{{ t('cookies.__info') }}</p>
  </button>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import { useCookies } from '@/core/adapter/cookies';

const { t } = useI18n();
const cookies = useCookies();

const manageCookies = () => {
  cookies.popupOpen = true;
};
</script>
