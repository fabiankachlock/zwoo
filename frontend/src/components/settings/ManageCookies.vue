<template>
  <div
    class="flex justify-center items-center bg-light border-2 border-transparent px-2 rounded transition hover:bg-main cursor-pointer select-none"
    @click="manageCookies"
  >
    <p class="tc-main-secondary text-center">{{ t('cookies.manage') }}</p>
  </div>
</template>

<script setup lang="ts">
import { useCookies } from '@/core/adapter/cookies';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const cookies = useCookies();

const manageCookies = () => {
  cookies.popupOpen = true;
};
</script>
