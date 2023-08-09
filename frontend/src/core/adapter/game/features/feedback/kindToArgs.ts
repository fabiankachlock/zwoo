import { ZRPFeedback, ZRPFeedbackKind } from '@/core/domain/zrp/zrpTypes';

export const getFeedbackArgsKeysForKind = (kind: ZRPFeedbackKind): (keyof ZRPFeedback['args'])[] => {
  switch (kind) {
    case ZRPFeedbackKind.Individual:
      return ['target'];
    case ZRPFeedbackKind.Unaffected:
      return [];
    case ZRPFeedbackKind.Interaction:
      return ['target', 'origin'];
  }
};
