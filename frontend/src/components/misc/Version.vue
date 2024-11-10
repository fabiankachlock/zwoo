<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { useRootApp } from '@/core/adapter/app';

const { t } = useI18n();

const clientInfo = useRootApp().versionInfo;
const serverInfo = computed(() => {
  const version = useRootApp().serverVersion;
  return 'callback' in version ? undefined : version;
});
</script>

<template>
  <div class="w-full my-2">
    <p class="text-text-secondary text-right text-sm italic">
      <span>{{ t('version.client') }}: </span>
      <span> {{ clientInfo.version }} ({{ clientInfo.hash }})</span>
    </p>
    <p v-if="serverInfo" class="text-text-secondary text-right text-sm italic">
      <span>{{ t('version.server') }}: </span>
      <span> {{ serverInfo?.version }} ({{ serverInfo?.hash }})</span>
    </p>
    <p class="text-text-secondary text-right text-sm italic">
      <span>{{ t('version.modeTitle') }}: </span>
      <span> {{ t(`version.mode.${clientInfo.mode}`) }}</span>
    </p>
  </div>
</template>
