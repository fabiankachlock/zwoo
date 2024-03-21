import { watch } from 'vue';

import { useGameFeedback } from '@/core/adapter/game/feedback';
import { ZRPFeedback, ZRPFeedbackType } from '@/core/domain/zrp/zrpTypes';

import { FeedbackConsumerReason, shouldShowFeedback } from './feedbackConfig';

export const useFeedback = (reason: FeedbackConsumerReason, handler: (feedback: ZRPFeedback) => void) => {
  const feedback = useGameFeedback();
  return watch(
    () => feedback.lastFeedback,
    feedback => {
      if (feedback && shouldShowFeedback(feedback, reason)) {
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
