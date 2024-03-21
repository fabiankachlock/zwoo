import { computed } from 'vue';

import { useRules } from '../rules';

export const useIsRuleActive = (key: string) => {
  const rules = useRules();
  return computed(() => (rules.rules.find(r => r.id === key)?.value ?? 0) > 0);
};
