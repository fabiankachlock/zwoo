<script setup lang="ts">
import { useI18n } from 'vue-i18n';

import { useLocalServer } from '@/core/adapter/tauri/localServer';

const { t } = useI18n();
const server = useLocalServer();

const logIn = () => {
  import('@tauri-apps/api/core').then(({ invoke }) => {
    invoke('open_url', { url: `http://${server.config.ip || '127.0.0.1'}:${server.config.port}/login-local` }).then(() => {
      console.log('success');
    });
  });
};
</script>

<template>
  <button
    class="flex justify-center items-center bg-bg border-2 border-transparent px-2 rounded transition hover:bg-bg cursor-pointer select-none"
    @click="logIn"
  >
    <p class="text-text-light text-center">{{ t('localServer.logIn') }}</p>
  </button>
</template>
