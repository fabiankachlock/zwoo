import { defineStore } from 'pinia';

import { ZRPMessage } from '@/core/domain/zrp/zrpTypes';
import Logger from '@/core/services/logging/logImport';

type QueueItem = {
  predicate: (msg: ZRPMessage) => boolean;
  msg: ZRPMessage;
};

export const useGameEvents = defineStore('game-events', {
  state: () => ({
    lastEvent: undefined as ZRPMessage | undefined,
    stack: [] as ZRPMessage[],
    queue: [] as QueueItem[]
  }),
  actions: {
    handleIncomingEvent(msg: ZRPMessage) {
      Logger.Zrp.log(`[incoming] ${msg.code} ${JSON.stringify(msg.data)}`);
      this.pushEvent(msg);
    },
    pushEvent(evt: ZRPMessage) {
      this.stack.push(evt);
      this.lastEvent = evt;

      const triggers = this.queue.filter(item => item.predicate(evt));
      this.queue = this.queue.filter(item => !item.predicate(evt));
      triggers.forEach(item => {
        queueMicrotask(() => {
          this.pushEvent(item.msg);
        });
      });
    },
    clear() {
      this.lastEvent = undefined;
      this.stack = [];
    },
    pushAfter(predicate: (msg: ZRPMessage) => boolean, msg: ZRPMessage) {
      this.queue.push({
        predicate: predicate,
        msg
      });
    },
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  }
});
