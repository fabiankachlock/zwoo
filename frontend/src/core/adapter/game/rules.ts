import { defineStore } from 'pinia';
import { ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { SettingsType, ZRPOPCode } from '@/core/domain/zrp/zrpTypes';

import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type DisplayRule = {
  id: string;
  title: Record<string, string>;
  description: Record<string, string>;
  value: number;
  isReadonly: boolean;
  ruleType: SettingsType;
  min?: number;
  max?: number;
};

const settingsWatcher = new MonolithicEventWatcher(ZRPOPCode.AllSettings, ZRPOPCode.SettingChanged);

export const useRules = defineStore('game-rules', () => {
  const rules = ref<DisplayRule[]>([]);
  const dispatchEvent = useGameEventDispatch();

  const _receiveMessage: (typeof settingsWatcher)['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.AllSettings) {
      rules.value = msg.data.settings.map(setting => ({
        id: setting.setting,
        title: setting.title, // `rules.${setting.setting}.title`,
        description: setting.description, //`rules.${setting.setting}.info`,
        value: setting.value,
        isReadonly: setting.isReadonly,
        ruleType: setting.type,
        min: setting.min,
        max: setting.max
      }));
    } else if (msg.code === ZRPOPCode.SettingChanged) {
      for (const setting of rules.value) {
        if (setting.id === msg.data.setting) {
          setting.value = msg.data.value;
          break;
        }
      }
    }
  };

  const updateRule = <K extends string>(key: K, value: number): void => {
    dispatchEvent(ZRPOPCode.UpdateSetting, {
      setting: key,
      value: value
    });
  };

  const setup = () => {
    dispatchEvent(ZRPOPCode.GetAllSettings, {});
  };

  const reset = () => {
    rules.value = [];
  };

  settingsWatcher.onOpen(setup);
  settingsWatcher.onMessage(_receiveMessage);
  settingsWatcher.onReset(() => {
    reset();
    setup();
  });
  settingsWatcher.onClose(reset);

  return {
    rules,
    updateRule,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
