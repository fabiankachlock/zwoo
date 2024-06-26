import { invoke } from '@tauri-apps/api/core';
import { defineStore } from 'pinia';
import { reactive, ref } from 'vue';

import { LocalServerConfig, LocalServerConfigManager } from '@/core/domain/localServer/localServerConfig';

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

  async function getUrl() {
    const newConfig = await LocalServerConfigManager.load();
    Object.assign(config, newConfig);

    let listeningPort = config.port == 0 ? 8001 : config.port;
    if (config.useDynamicPort) listeningPort = 0;

    let listeningIp = '0.0.0.0';
    if (config.useAllIps) listeningIp = '127.0.0.1';
    else if (config.useLocalhost) listeningIp = '127.0.0.1';
    else listeningIp = config.ip;
    return `http://${listeningIp}:${listeningPort}/api/`;
  }

  const saveConfig = (newValue: LocalServerConfig) => {
    LocalServerConfigManager.save(newValue);
    Object.assign(config, newValue);
  };

  return {
    isRunning,
    config,
    startServer,
    stopServer,
    loadStatus,
    saveConfig,
    getUrl
  };
});
