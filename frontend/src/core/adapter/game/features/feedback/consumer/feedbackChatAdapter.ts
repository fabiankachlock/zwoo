import { defineStore } from 'pinia';

import { ZRPFeedback } from '@/core/domain/zrp/zrpTypes';
import { ZRPRole } from '@/core/domain/zrp/zrpTypes';
import { I18nInstance } from '@/i18n';

import { useChatStore } from '../../../chat';
import { usePlayerManager } from '../../../playerManager';
import { resolveFeedbackArgs } from '../argsHelper';
import { FeedbackConsumerReason } from '../feedbackConfig';
import { FeedbackWatcher } from '../FeedbackWatcher';

const chatFeedbackWatcher = new FeedbackWatcher(FeedbackConsumerReason.Chat);

export const useFeedbackChatAdapter = defineStore('feedback-to-chat', () => {
  const chat = useChatStore();
  const players = usePlayerManager();

  const resolveTranslatedFeedback = (feedback: ZRPFeedback) => {
    return I18nInstance.t(
      `feedback.chat.${feedback.type}`,
      resolveFeedbackArgs(feedback, id => players.getPlayerName(id), true)
    );
  };

  chatFeedbackWatcher.onMessage(feedback => {
    chat._pushMessage(resolveTranslatedFeedback(feedback), {
      id: 0,
      role: ZRPRole._System,
      name: ''
    });
  });

  return {
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
