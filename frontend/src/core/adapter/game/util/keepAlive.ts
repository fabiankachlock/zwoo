import { defineStore } from 'pinia';

import { MonolithicEventWatcher } from '@/core/adapter/game/util/MonolithicEventWatcher';
import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { ZRPOPCode } from '@/core/domain/zrp/zrpTypes';
import Logger from '@/core/services/logging/logImport';

const keepAliveWatcher = new MonolithicEventWatcher(ZRPOPCode.AckKeepAlive);

export const useKeepAlive = defineStore('game-keep-alive', () => {
  const send = useGameEventDispatch();
  let timeout: number | undefined = undefined;

  const _receiveMessage: typeof keepAliveWatcher['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.AckKeepAlive) {
      Logger.debug('acknowledged keep alive');
    }
  };

  keepAliveWatcher.onOpen(() => {
    if (timeout) {
      clearInterval(timeout);
    }
    timeout = setInterval(() => {
      Logger.debug('sending keep alive');
      send(ZRPOPCode.KeepAlive, {});
    }, 1_000 * 20);
  });

  keepAliveWatcher.onClose(() => {
    if (timeout) {
      clearInterval(timeout);
      timeout = undefined;
    }
  });

  keepAliveWatcher.onMessage(_receiveMessage);

  return {
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
