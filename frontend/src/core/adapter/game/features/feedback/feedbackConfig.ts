import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { useGameConfig } from '@/core/adapter/game';
// import { useGameConfig } from '@/core/adapter/game';
import { ZRPFeedback } from '@/core/domain/zrp/zrpTypes';

export enum FeedbackConsumerReason {
  Chat = 'chat'
}

export enum FeedbackConsumingRange {
  OnlySelf = 'self',
  All = 'all',
  None = 'none'
}

export const isFeedbackReasonEnabled = (reason: FeedbackConsumerReason) => {
  const config = useConfig();
  const consumers = getAllowedConsumers(config.get(ZwooConfigKey.FeedbackDisplay));
  return consumers.includes(reason);
};

export const shouldShowFeedback = (feedback: ZRPFeedback) => {
  const config = useConfig();
  const { lobbyId } = useGameConfig();

  const range = getFeedbackRange(config.get(ZwooConfigKey.FeedbackRange));
  if (range === FeedbackConsumingRange.All) {
    return true;
  } else if (range === FeedbackConsumingRange.OnlySelf) {
    return feedback.args.target === lobbyId || feedback.args.origin === lobbyId;
  }
  // range === FeedbackConsumingRange.None
  return false;
};

export const getAllowedConsumers = (storedConfig: string): FeedbackConsumerReason[] => {
  const allowed: string[] = Object.values(FeedbackConsumerReason);
  return storedConfig
    .split(',')
    .map(consumer => (allowed.includes(consumer) ? (consumer as FeedbackConsumerReason) : (null as unknown as FeedbackConsumerReason)))
    .filter(consumer => consumer);
};

export const getFeedbackRange = (storedConfig: string): FeedbackConsumingRange => {
  switch (storedConfig) {
    case FeedbackConsumingRange.All:
      return FeedbackConsumingRange.All;
    case FeedbackConsumingRange.OnlySelf:
      return FeedbackConsumingRange.OnlySelf;
    case FeedbackConsumingRange.None:
      return FeedbackConsumingRange.None;
    default:
      return FeedbackConsumingRange.OnlySelf;
  }
};

export const stringifyFeedbackRange = (range: FeedbackConsumingRange): string => range;

export const stringifyFeedbackConsumer = (consumers: FeedbackConsumerReason[]): string => consumers.join(',');
