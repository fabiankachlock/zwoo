import { useConfig } from '@/core/adapter/config';
import { useGameConfig } from '@/core/adapter/game';
import { ZRPFeedback } from '@/core/domain/zrp/zrpTypes';
import { I18nInstance } from '@/i18n';

import { usePlayerManager } from '../../playerManager';
import { resolveFeedbackArgs } from './argsHelper';
import { FeedbackConsumerReason, FeedbackConsumingRange, getConfigKeyForFeedbackReason, getFeedbackRange } from './feedbackConfig';

export const getFeedbackTranslator =
  (reason: FeedbackConsumerReason) =>
  (feedback: ZRPFeedback): string => {
    const { lobbyId } = useGameConfig();
    const playerManager = usePlayerManager();
    const configKey = getConfigKeyForFeedbackReason(reason);
    if (!configKey) return '';

    const config = useConfig().get(configKey);
    const range = getFeedbackRange(config as FeedbackConsumingRange);

    const args = resolveFeedbackArgs(feedback, idToResolve => {
      if (range === FeedbackConsumingRange.OnlySelf && lobbyId === idToResolve) {
        return I18nInstance.t(`feedback.selfName`);
      }
      return playerManager.getPlayerName(idToResolve);
    });
    return I18nInstance.t(`feedback.${feedback.type}`, args);
  };
