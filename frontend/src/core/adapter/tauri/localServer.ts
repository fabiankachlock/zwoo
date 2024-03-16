import { invoke } from '@tauri-apps/api';
import { defineStore } from 'pinia';
import { reactive, ref, watch } from 'vue';

import { LocalServerConfigManager } from '@/core/domain/localServer/localServerConfig';

export const useLocalServer = defineStore('localServer', () => {
  const isRunning = ref(false);
  const config = reactive(LocalServerConfigManager.defaultConfig);

  invoke<boolean>('get_server_status').then(status => {
    isRunning.value = status;
  });

  LocalServerConfigManager.load().then(initialConfig => {
    Object.assign(config, initialConfig);
  });

  async function startServer() {
    await invoke('start_local_server');
    invoke<boolean>('get_server_status').then(status => {
      isRunning.value = status;
    });
  }

  async function stopServer() {
    await invoke('stop_local_server');
    invoke<boolean>('get_server_status').then(status => {
      isRunning.value = status;
    });
  }

  async function loadStatus() {
    invoke<boolean>('get_server_status').then(status => {
      isRunning.value = status;
    });
  }

  watch(config, newValue => {
    LocalServerConfigManager.save(newValue);
  });

  return {
    isRunning,
    config,
    startServer,
    stopServer,
    loadStatus
  };
});
