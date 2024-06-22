<template>
  <div v-if="updateExists" class="w-full my-2 mb-4">
    <div class="bg-surface rounded-lg p-2">
      <div class="flex justify-between items-center">
        <p class="text-text">{{ t('settings.update.available') }}</p>
        <div class="bg-alt hover:bg-alt-hover border border-border rounded px-2 py-0.5 text-text">
          <div v-if="!isReady" class="flex flex-row justify-center flex-nowrap items-center text-text select-none cursor-pointer">
            <ZwooIcon icon="iconoir:system-restart" class="text-xl animate-spin-slow mr-2" />
            <p class="text-lg">{{ t('settings.update.downloading') }}</p>
          </div>
          <button v-else @click="reload">{{ t('settings.update.updateNow') }}</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { useRootApp } from '@/core/adapter/app';
import ZwooIcon from '@/modules/zwoo-icons/ZwooIcon.vue';

const app = useRootApp();
const { t } = useI18n();

const updateExists = computed(() => !app.serverVersionMatches);
const isReady = computed(() => app.updateAvailable);

const reload = () => {
  if (app.updateAvailable) {
    app.updateApp();
  }
};
</script>
