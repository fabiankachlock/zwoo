export const AllRules = ['maxPlayers', 'initialCards'];

export enum RuleType {
  Numeric,
  Boolean
}

export const EditableRules = [...AllRules] as string[]; // AllRules; TODO: Allow after beta

export const RuleTypeDefinitions: Record<typeof AllRules[number], RuleType> = {
  maxPlayers: RuleType.Numeric,
  initialCards: RuleType.Numeric
};
