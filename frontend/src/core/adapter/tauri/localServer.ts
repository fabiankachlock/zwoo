import { defineStore } from 'pinia';

import { LocalServerConfig } from '@/core/domain/localServer/localServerConfig';

export const useLocalServer = defineStore('localServer', {
  state: () => ({
    isRunning: false,
    isStarting: false,
    config: {
      serverId: 'serverID',
      port: 123,
      ip: '123',
      useDynamicPort: false,
      useLocalhost: true,
      useAllIPs: false,
      useStrictOrigins: true,
      allowedOrigins: ['a', 'b']
    } as LocalServerConfig
    //undefined as LocalServerConfig | undefined
  }),
  actions: {
    startServer() {
      // empty
    },
    stopServer() {
      // empty
    }
  }
});
