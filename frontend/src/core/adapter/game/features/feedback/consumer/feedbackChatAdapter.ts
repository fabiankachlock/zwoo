import { defineStore } from 'pinia';

import { ZRPRole } from '@/core/domain/zrp/zrpTypes';

import { useChatStore } from '../../../chat';
import { FeedbackConsumerReason } from '../feedbackConfig';
import { FeedbackWatcher } from '../FeedbackWatcher';
import { getFeedbackTranslator } from '../i18nHelper';

const chatFeedbackWatcher = new FeedbackWatcher(FeedbackConsumerReason.Chat);

export const useFeedbackChatAdapter = defineStore('feedback-to-chat', () => {
  const chat = useChatStore();
  const translate = getFeedbackTranslator();

  chatFeedbackWatcher.onMessage(feedback => {
    chat._pushMessage(translate(feedback), {
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
