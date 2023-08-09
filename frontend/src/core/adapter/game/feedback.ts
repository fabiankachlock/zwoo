import { defineStore } from 'pinia';
import { nextTick, ref } from 'vue';

import { ZRPFeedback, ZRPOPCode } from '@/core/domain/zrp/zrpTypes';

import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

const feedbackWatcher = new MonolithicEventWatcher(ZRPOPCode.StateUpdate);

export const useGameFeedback = defineStore('game-feedback', () => {
  const lastFeedback = ref<ZRPFeedback | undefined>(undefined);

  feedbackWatcher.onMessage(async msg => {
    if (msg.code === ZRPOPCode.StateUpdate) {
      iterate(msg.data.feedback);
    }
  });

  const iterate = (feedback: ZRPFeedback[]) => {
    if (feedback.length === 0) return;
    nextTick(() => {
      lastFeedback.value = feedback.shift();
      iterate(feedback);
    });
  };

  const reset = () => {
    lastFeedback.value = undefined;
  };

  feedbackWatcher.onReset(reset);
  feedbackWatcher.onClose(reset);

  return {
    lastFeedback,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
