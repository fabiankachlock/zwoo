<template>
  <button
    class="flex justify-center items-center bg-bg border-2 border-transparent px-2 rounded transition hover:bg-bg cursor-pointer select-none"
    @click="seeVersionHistory"
  >
    <p class="text-text text-center">{{ t('versionHistory.see') }}</p>
  </button>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

const { t } = useI18n();
const router = useRouter();

const seeVersionHistory = () => {
  router.push('/version-history');
};
</script>
