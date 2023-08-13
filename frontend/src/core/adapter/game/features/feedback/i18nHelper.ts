import { useGameConfig } from '@/core/adapter/game';
import { ZRPFeedback, ZRPFeedbackKind } from '@/core/domain/zrp/zrpTypes';
import { I18nInstance } from '@/i18n';

import { usePlayerManager } from '../../playerManager';
import { feedbackIncludesPlayer, resolveFeedbackArgs } from './argsHelper';

export const getFeedbackTranslator =
  () =>
  (feedback: ZRPFeedback): string => {
    const { lobbyId } = useGameConfig();
    const playerManager = usePlayerManager();
    if (!lobbyId) return '';

    const args = resolveFeedbackArgs(feedback, idToResolve => {
      if (lobbyId === idToResolve) {
        return I18nInstance.t(`feedback.selfName`);
      }
      return playerManager.getPlayerName(idToResolve);
    });

    // resolve message modifier based on own position in the feedback
    let i18nKeyModifier = '';
    if (feedbackIncludesPlayer(feedback, lobbyId)) {
      i18nKeyModifier = '-self';
      if (feedback.kind === ZRPFeedbackKind.Interaction) {
        if (feedback.args.target === lobbyId) {
          args['other'] = args.origin;
          i18nKeyModifier += '+target';
        } else {
          args['other'] = args.target;
          i18nKeyModifier += '+origin';
        }
      }
    }

    // try to get specific message
    const translated = I18nInstance.t(`feedback.${feedback.type}${i18nKeyModifier}`, args);
    // if its not available - send generic one
    if (translated === `feedback.${feedback.type}${i18nKeyModifier}`) return I18nInstance.t(`feedback.${feedback.type}`, args);
    return translated;
  };
