import { defineStore } from 'pinia';
import { ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/play/util/useGameEventDispatch';
import { ZRPBotConfig, ZRPOPCode } from '@/core/services/zrp/zrpTypes';

import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

const botsWatcher = new MonolithicEventWatcher(ZRPOPCode.ListBots, ZRPOPCode.PromotedToHost);

export const useBotManager = defineStore('bot-manager', () => {
  const bots = ref<Record<string, { name: string; config: ZRPBotConfig }>>({});
  const dispatchEvent = useGameEventDispatch();

  const _receiveMessage: typeof botsWatcher['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.ListBots) {
      bots.value = msg.data.bots.reduce(
        (map, bot) => ({
          [bot.username]: {
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
    bots.value[name] = {
      name: name,
      config: config
    };
  };

  const updateBot = (name: string, config: ZRPBotConfig) => {
    dispatchEvent(ZRPOPCode.CreateBot, { username: name, config: config });
    bots.value[name] = {
      name: name,
      config: config
    };
  };

  const deleteBot = (name: string) => {
    dispatchEvent(ZRPOPCode.DeleteBot, { username: name });
    delete bots.value[name];
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
