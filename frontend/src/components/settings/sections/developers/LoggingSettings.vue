<template>
  <div>
    <button
      class="flex justify-center items-center bg-bg border-2 border-transparent px-2 rounded transition hover:bg-bg cursor-pointer select-none"
      @click="router.push('/__dev__/logging')"
    >
      <p class="text-text text-center">{{ t('settings.goToLogging') }}</p>
    </button>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

const { t } = useI18n();
const router = useRouter();
</script>
