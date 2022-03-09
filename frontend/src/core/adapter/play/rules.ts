import { AllRules, DefaultRuleConfig } from '@/core/services/game/rules';
import { defineStore } from 'pinia';

export type DisplayRule = {
  id: string;
  title: string;
  description: string;
  isActivated: boolean;
};

export const useRules = defineStore('rules', {
  state: () => ({
    rules: AllRules.map(id => ({
      id: id,
      title: `rules.${id}.title`,
      description: `rules.${id}.info`,
      isActivated: DefaultRuleConfig[id]
    })) as DisplayRule[]
  }),

  actions: {
    toggleRule(ruleId: string, isActive: boolean): void {
      this.rules = this.rules.map(rule => ({
        ...rule,
        isActivated: rule.id === ruleId ? isActive : rule.isActivated
      }));
    }
  }
});
