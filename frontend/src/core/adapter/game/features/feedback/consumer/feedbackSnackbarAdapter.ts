import { defineStore } from 'pinia';

import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';

import { FeedbackConsumerReason } from '../feedbackConfig';
import { FeedbackWatcher } from '../FeedbackWatcher';
import { getFeedbackTranslator } from '../i18nHelper';

const snackbarFeedbackWatcher = new FeedbackWatcher(FeedbackConsumerReason.Snackbar);

export const useFeedbackSnackbarAdapter = defineStore('feedback-to-snackbar', () => {
  const snackbar = useSnackbar();
  const translate = getFeedbackTranslator();

  snackbarFeedbackWatcher.onMessage(feedback => {
    snackbar.pushMessage({
      message: translate(feedback),
      needsTranslation: false,
      position: SnackBarPosition.Top,
      duration: 1000,
      hideProgress: true,
      showClose: false
    });
  });

  return {
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
