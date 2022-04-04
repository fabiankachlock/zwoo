import Logger from '@/core/services/logging/logImport';
import { ZRPMessage } from '@/core/services/zrp/zrpTypes';
import { defineStore } from 'pinia';

export const useGameEvents = defineStore('game-events', {
  state: () => ({
    lastEvent: undefined as ZRPMessage | undefined,
    stack: [] as ZRPMessage[]
  }),
  actions: {
    handleIncomingEvent(msg: ZRPMessage) {
      Logger.Zrp.log(`[incoming] ${msg.code} ${msg.data}`);
      this.lastEvent = msg;
      this.stack.push(msg);
    }
  }
});
