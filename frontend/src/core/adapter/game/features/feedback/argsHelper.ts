import { ZRPFeedback, ZRPFeedbackKind } from '@/core/domain/zrp/zrpTypes';

export const getFeedbackArgsKeysForKind = (kind: ZRPFeedbackKind): string[] => {
  switch (kind) {
    case ZRPFeedbackKind.Individual:
      return ['target'];
    case ZRPFeedbackKind.Unaffected:
      return [];
    case ZRPFeedbackKind.Interaction:
      return ['target', 'origin'];
  }
};

export const resolveFeedbackArgs = <stringify extends boolean>(
  feedback: ZRPFeedback,
  resolveName: (lobbyId: number) => string,
  stringifyAll?: stringify
): Record<string, stringify extends true ? string : string | number> => {
  const keys = getFeedbackArgsKeysForKind(feedback.kind);
  const extraKeys = Object.keys(feedback.args).filter(key => !keys.includes(key));

  const targetArgs: Record<string, number> = keys.reduce(
    (args, currentKey) => ({
      ...args,
      [currentKey]: feedback.args[currentKey]
    }),
    {}
  );

  const resolvedArgs: Record<string, string | number> = {};
  for (const key in targetArgs) {
    resolvedArgs[key] = resolveName(feedback.args[key]);
  }
  for (const key in extraKeys) {
    if (stringifyAll) {
      resolvedArgs[key] = feedback.args[key].toString();
    } else {
      resolvedArgs[key] = feedback.args[key];
    }
  }
  return resolvedArgs as Record<string, stringify extends true ? string : string | number>;
};
