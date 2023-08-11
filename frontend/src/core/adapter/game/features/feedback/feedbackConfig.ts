import { useConfig, ZwooConfigKey } from '@/core/adapter/config';
import { useGameConfig } from '@/core/adapter/game';
// import { useGameConfig } from '@/core/adapter/game';
import { ZRPFeedback } from '@/core/domain/zrp/zrpTypes';

export enum FeedbackConsumerReason {
  Chat = 'chat',
  Snackbar = 'snackbar'
}

export enum FeedbackConsumingRange {
  All = 'all',
  OnlySelf = 'self',
  None = 'none'
}

export const getConfigKeyForFeedbackReason = (reason: FeedbackConsumerReason): ZwooConfigKey => {
  switch (reason) {
    case FeedbackConsumerReason.Chat:
      return ZwooConfigKey.FeedbackChat;
    case FeedbackConsumerReason.Snackbar:
      return ZwooConfigKey.FeedbackSnackbar;
  }
};

export const shouldShowFeedback = (feedback: ZRPFeedback, reason: FeedbackConsumerReason) => {
  const configKey = getConfigKeyForFeedbackReason(reason);
  if (!configKey) return false;

  const config = useConfig().get(configKey);
  const { lobbyId } = useGameConfig();

  const range = getFeedbackRange(config as FeedbackConsumingRange);
  if (range === FeedbackConsumingRange.All) {
    return true;
  } else if (range === FeedbackConsumingRange.OnlySelf) {
    return feedback.args.target === lobbyId || feedback.args.origin === lobbyId;
  }
  // range === FeedbackConsumingRange.None
  return false;
};

export const getFeedbackRange = (storedConfig: string | undefined): FeedbackConsumingRange => {
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
