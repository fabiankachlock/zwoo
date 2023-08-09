import { defineStore } from 'pinia';

import { ZRPFeedback } from '@/core/domain/zrp/zrpTypes';
import { ZRPRole } from '@/core/domain/zrp/zrpTypes';
import { I18nInstance } from '@/i18n';

import { useChatStore } from '../../../chat';
import { usePlayerManager } from '../../../playerManager';
import { FeedbackConsumerReason } from '../feedbackConfig';
import { FeedbackWatcher } from '../FeedbackWatcher';
import { getFeedbackArgsKeysForKind } from '../kindToArgs';

const chatFeedbackWatcher = new FeedbackWatcher(FeedbackConsumerReason.Chat);

export const useFeedbackChatAdapter = defineStore('feedback-to-chat', () => {
  const chat = useChatStore();
  const players = usePlayerManager();

  const aggregateArgs = (feedback: ZRPFeedback): Record<string, string> => {
    const keys = getFeedbackArgsKeysForKind(feedback.kind);
    const targetArgs: Record<string, number> = keys.reduce(
      (args, currentKey) => ({
        ...args,
        [currentKey]: feedback.args[currentKey]
      }),
      {}
    );

    const resolvedArgs: Record<string, string> = {};
    for (const key in targetArgs) {
      resolvedArgs[key] = players.getPlayerName(targetArgs[key] as number);
    }
    return resolvedArgs;
  };

  const resolveTranslatedFeedback = (feedback: ZRPFeedback) => {
    return I18nInstance.t(`feedback.chat.${feedback.type}`, aggregateArgs(feedback));
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
