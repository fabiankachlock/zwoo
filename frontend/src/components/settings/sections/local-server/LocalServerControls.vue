<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { useLocalServer } from '@/core/adapter/tauri/localServer';

const { t } = useI18n();
const server = useLocalServer();
const isRunning = computed(() => server.isRunning);

const toggleServer = () => {
  if (!isRunning.value) {
    server.startServer();
  } else {
    server.stopServer();
  }
};
</script>

<template>
  <button
    class="flex justify-center items-center bg-bg border-2 border-transparent px-2 rounded transition hover:bg-bg cursor-pointer select-none"
    v-if="!isRunning"
    @click="toggleServer"
  >
    <p class="text-text text-center">{{ t('localServer.start') }}</p>
  </button>
  <button
    class="flex justify-center items-center bg-bg border-2 border-transparent px-2 rounded transition hover:bg-bg cursor-pointer select-none"
    v-else
    @click="toggleServer"
  >
    <p class="text-text text-center">{{ t('localServer.stop') }}</p>
  </button>
</template>
