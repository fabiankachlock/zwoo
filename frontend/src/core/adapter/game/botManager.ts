import { defineStore } from 'pinia';
import { ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { ZRPBotConfig, ZRPOPCode } from '@/core/domain/zrp/zrpTypes';

import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type BotType = { id: number; name: string; config: ZRPBotConfig };

const botsWatcher = new MonolithicEventWatcher(ZRPOPCode.ListBots, ZRPOPCode.PromotedToHost);

export const useBotManager = defineStore('bot-manager', () => {
  const bots = ref<Record<number, BotType>>({});
  const dispatchEvent = useGameEventDispatch();

  const _receiveMessage: (typeof botsWatcher)['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.ListBots) {
      bots.value = msg.data.bots.reduce(
        (map, bot) => ({
          [bot.id]: {
            id: bot.id,
            name: bot.username,
            config: bot.config
          },
          ...map
        }),
        {}
      );
    } else if (msg.code === ZRPOPCode.PromotedToHost) {
      setup();
    }
  };

  const addBot = (name: string, config: ZRPBotConfig) => {
    dispatchEvent(ZRPOPCode.CreateBot, { username: name, config: config });
  };

  const updateBot = (id: number, config: ZRPBotConfig) => {
    dispatchEvent(ZRPOPCode.UpdateBot, { id: id, config: config });
    bots.value[id].config = config;
  };

  const deleteBot = (id: number) => {
    dispatchEvent(ZRPOPCode.DeleteBot, { id: id });
    delete bots.value[id];
  };

  const reset = () => {
    bots.value = {};
  };

  const setup = () => {
    dispatchEvent(ZRPOPCode.GetBots, {});
  };

  botsWatcher.onOpen(setup);
  botsWatcher.onMessage(_receiveMessage);
  botsWatcher.onReset(() => {
    reset();
    setup();
  });
  botsWatcher.onClose(reset);

  return {
    botConfigs: bots,
    addBot,
    updateBot,
    deleteBot,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
