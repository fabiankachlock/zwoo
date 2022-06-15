export const AllRules = ['maxPlayers', 'maxDraw', 'maxCardsOnHand', 'initialCards'];

export enum RuleType {
  Numeric,
  Boolean
}

export const EditableRules = [] as string[]; // AllRules; TODO: Allow after beta

export const RuleTypeDefinitions: Record<typeof AllRules[number], RuleType> = {
  maxPlayers: RuleType.Numeric,
  maxDraw: RuleType.Numeric,
  maxCardsOnHand: RuleType.Numeric,
  initialCards: RuleType.Numeric
};
