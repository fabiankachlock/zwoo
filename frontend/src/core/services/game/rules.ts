export const AllRules = ['test-1', 'key-2'] as const;

export const DefaultRuleConfig: Record<typeof AllRules[number], boolean> = {
  'test-1': false,
  'key-2': true
};
