import { defineStore } from 'pinia';
import { ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { SettingsType, ZRPOPCode, ZRPSettingsPayload } from '@/core/domain/zrp/zrpTypes';

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
  children: DisplayRule[];
};

const settingsWatcher = new MonolithicEventWatcher(ZRPOPCode.AllSettings, ZRPOPCode.SettingChanged);

export const useRules = defineStore('game-rules', () => {
  const rules = ref<DisplayRule[]>([]);
  const dispatchEvent = useGameEventDispatch();

  const _receiveMessage: (typeof settingsWatcher)['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.AllSettings) {
      rules.value = transformRules(msg.data.settings);
    } else if (msg.code === ZRPOPCode.SettingChanged) {
      for (const setting of rules.value) {
        if (setting.id === msg.data.setting) {
          setting.value = msg.data.value;
          break;
        } else if (msg.data.setting.startsWith(setting.id)) {
          for (const subSetting of setting.children) {
            if (subSetting.id === msg.data.setting) {
              subSetting.value = msg.data.value;
              break;
            }
          }
        }
      }
    }
  };

  const transformRules = (settings: ZRPSettingsPayload['settings']): DisplayRule[] => {
    const groups: Record<string, { root?: DisplayRule; children: DisplayRule[] }> = {};

    settings.forEach(setting => {
      const displayRule: DisplayRule = {
        id: setting.setting,
        title: setting.title, // `rules.${setting.setting}.title`,
        description: setting.description, //`rules.${setting.setting}.info`,
        value: setting.value,
        isReadonly: setting.isReadonly,
        ruleType: setting.type,
        min: setting.min,
        max: setting.max,
        children: []
      };

      const stem = displayRule.id.split('.')[0];
      if (stem === displayRule.id) {
        if (!groups[stem]) {
          groups[stem] = {
            root: displayRule,
            children: []
          };
        } else {
          groups[stem].root = displayRule;
        }
      } else if (displayRule.id.length > stem.length) {
        if (!groups[stem]) {
          groups[stem] = {
            root: undefined,
            children: [displayRule]
          };
        } else {
          groups[stem].children.push(displayRule);
        }
      }
    });

    return Object.values(groups)
      .filter(g => g.root !== undefined)
      .map(g => {
        if (g.root) {
          g.root.children = g.root.children.concat(g.children);
        }
        return g.root as DisplayRule;
      });
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
