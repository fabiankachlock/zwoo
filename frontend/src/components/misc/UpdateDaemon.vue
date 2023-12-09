<template>
  <div v-if="updateExists" class="w-full px-2 my-2 mb-4">
    <div class="bg-dark hover:bg-darkest rounded-lg px-3 py-3">
      <div class="flex justify-between items-center">
        <p class="tc-main">{{ t('settings.update.available') }}</p>
        <div class="bg-light hover:bg-main rounded px-2 py-1 tc-main">
          <div v-if="!isReady" class="flex flex-row justify-center flex-nowrap items-center tc-main">
            <ZwooIcon icon="iconoir:system-restart" class="text-xl tc-main-light animate-spin-slow mr-2" />
            <p class="text-lg tc-main-secondary">{{ t('settings.update.downloading') }}</p>
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
