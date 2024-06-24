<template>
  <div>
    <button
      class="flex justify-center items-center bg-alt border-2 border-transparent px-2 rounded transition hover:bg-alt-hover cursor-pointer select-none"
      @click="clearLocalLogs()"
    >
      <p class="text-text text-center">{{ t('settings.clearLogs') }}</p>
    </button>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';

const { t } = useI18n();
const { pushMessage } = useSnackbar();

const clearLocalLogs = async () => {
  const module = await import('@/core/services/logging/logImport');
  module.LogStore.reset();
  pushMessage({
    position: SnackBarPosition.Top,
    message: 'logging.logsCleared',
    needsTranslation: true
  });
};
</script>
