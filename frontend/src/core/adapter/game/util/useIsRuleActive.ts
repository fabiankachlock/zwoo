import { computed } from 'vue';

import { AllRules } from '@/core/domain/game/GameRules';

import { useRules } from '../rules';

export const useIsRuleActive = (key: (typeof AllRules)[number]) => {
  const rules = useRules();
  return computed(() => (rules.rules.find(r => r.id === key)?.value ?? 0) > 0);
};
