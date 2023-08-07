import { watch } from 'vue';

import { useGameFeedback } from '@/core/adapter/game/feedback';
import { ZRPFeedback, ZRPFeedbackType } from '@/core/domain/zrp/zrpTypes';

import { FeedbackConsumerReason, isFeedbackReasonEnabled, shouldShowFeedback } from './feedbackConfig';

export const useFeedbackRaw = (reason: FeedbackConsumerReason, handler: (feedback: ZRPFeedback) => void) => {
  const feedback = useGameFeedback();
  return watch(
    () => feedback.lastFeedback,
    feedback => {
      if (feedback && isFeedbackReasonEnabled(reason)) {
        handler(feedback);
      }
    }
  );
};

export const useFeedback = (reason: FeedbackConsumerReason, handler: (feedback: ZRPFeedback) => void) => {
  const feedback = useGameFeedback();
  return watch(
    () => feedback.lastFeedback,
    feedback => {
      if (feedback && shouldShowFeedback(feedback) && isFeedbackReasonEnabled(reason)) {
        handler(feedback);
      }
    }
  );
};

export const useFeedbackType = (reason: FeedbackConsumerReason, type: ZRPFeedbackType, handler: (feedback: ZRPFeedback) => void) => {
  return useFeedback(reason, feedback => {
    if (feedback.type === type) {
      handler(feedback);
    }
  });
};
