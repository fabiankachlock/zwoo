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
      Logger.Zrp.log(`[incoming] ${msg.code} ${JSON.stringify(msg.data)}`);
      this.pushEvent(msg);
    },
    pushEvent(evt: ZRPMessage) {
      this.stack.push(evt);
      this.lastEvent = evt;
    },
    clear() {
      this.lastEvent = undefined;
      this.stack = [];
    },
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  }
});
