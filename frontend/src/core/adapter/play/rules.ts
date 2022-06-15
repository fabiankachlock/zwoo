import { useGameEventDispatch } from '@/composables/eventDispatch';
import { AllRules, EditableRules, RuleType, RuleTypeDefinitions } from '@/core/services/game/rules';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type DisplayRule = {
  id: string;
  value: number;
  ruleType: RuleType;
  isReadonly: boolean;
  title: string;
  description: string;
};

const settingsWatcher = new MonolithicEventWatcher(ZRPOPCode.AllSettings, ZRPOPCode.SettingsUpdated);

export const useRules = defineStore('game-rules', () => {
  const rules = ref<DisplayRule[]>([]);
  const dispatchEvent = useGameEventDispatch();

  const _receiveMessage: typeof settingsWatcher['_msgHandler'] = msg => {
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
    } else if (msg.code === ZRPOPCode.SettingsUpdated) {
      for (const setting of rules.value) {
        if (setting.id === msg.data.setting) {
          setting.value = msg.data.value;
          break;
        }
      }
    }
  };

  const updateRule = <K extends string>(key: K, value: number): void => {
    dispatchEvent(ZRPOPCode.ChangeSettings, {
      setting: key,
      value: value
    });
  };

  const setup = () => {
    dispatchEvent(ZRPOPCode.GetAllSettings, {});
  };

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  const reset = () => {};

  settingsWatcher.onMessage(_receiveMessage);
  settingsWatcher.onClose(reset);
  settingsWatcher.onOpen(setup);

  return {
    rules,
    updateRule,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
