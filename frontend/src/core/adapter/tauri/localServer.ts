import { invoke } from '@tauri-apps/api';
import { defineStore } from 'pinia';
import { reactive, ref, watch } from 'vue';

import { LocalServerConfigManager } from '@/core/domain/localServer/localServerConfig';

export const useLocalServer = defineStore('localServer', () => {
  const isRunning = ref(false);
  const config = reactive(LocalServerConfigManager.defaultConfig);

  LocalServerConfigManager.load().then(initialConfig => {
    Object.assign(config, initialConfig);
  });

  function startServer() {
    invoke('start_local_server').then(() => console.log('Server started'));
  }

  function stopServer() {
    invoke('stop_local_server').then(() => console.log('Server started'));
  }

  watch(config, newValue => {
    LocalServerConfigManager.save(newValue);
  });

  return {
    isRunning,
    config,
    startServer,
    stopServer
  };
});
