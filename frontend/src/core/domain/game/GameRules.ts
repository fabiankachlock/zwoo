export const AllRules = ['maxPlayers', 'initialCards', 'addUpDraw', 'explicitLastCard'];

export enum RuleType {
  Numeric,
  Boolean
}

export const EditableRules = [...AllRules] as string[]; // AllRules;

export const RuleTypeDefinitions: Record<(typeof AllRules)[number], RuleType> = {
  maxPlayers: RuleType.Numeric,
  initialCards: RuleType.Numeric,
  addUpDraw: RuleType.Boolean,
  explicitLastCard: RuleType.Boolean
};
