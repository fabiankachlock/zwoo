import { useConfig } from '@/core/adapter/config';
import { ZRPFeedback } from '@/core/domain/zrp/zrpTypes';

export enum FeedbackConsumerReason {
  Chat = 'chat'
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export const isFeedbackReasonEnabled = (reason: FeedbackConsumerReason) => {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const config = useConfig();
  // return config.get()
  return true;
};

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export const shouldShowFeedback = (feedback: ZRPFeedback) => {
  // TODO: implement feedback guards like `onlySelf` or `all`
  return true;
};
