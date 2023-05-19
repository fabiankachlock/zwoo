import { defineStore } from 'pinia';
import { ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { AllRules, EditableRules, RuleType, RuleTypeDefinitions } from '@/core/domain/game/GameRules';
import { ZRPOPCode } from '@/core/domain/zrp/zrpTypes';

import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type DisplayRule = {
  id: string;
  value: number;
  ruleType: RuleType;
  isReadonly: boolean;
  title: string;
  description: string;
};

const settingsWatcher = new MonolithicEventWatcher(ZRPOPCode.AllSettings, ZRPOPCode.SettingChanged);

export const useRules = defineStore('game-rules', () => {
  const rules = ref<DisplayRule[]>([]);
  const dispatchEvent = useGameEventDispatch();

  const _receiveMessage: (typeof settingsWatcher)['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.AllSettings) {
      rules.value = msg.data.settings
        .filter(setting => AllRules.includes(setting.setting))
        .map(setting => ({
          id: setting.setting,
          title: `rules.${setting.setting}.title`,
          description: `rules.${setting.setting}.info`,
          value: setting.value,
          isReadonly: !EditableRules.includes(setting.setting),
          ruleType: RuleTypeDefinitions[setting.setting] as RuleType
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
