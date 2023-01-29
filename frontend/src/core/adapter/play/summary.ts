import { defineStore } from 'pinia';
import { ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/play/util/useGameEventDispatch';
import { RouterService } from '@/core/services/global/Router';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';

import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type GameSummaryEntry = {
  id: string;
  username: string;
  score: number;
  position: number;
};

const summaryWatcher = new MonolithicEventWatcher(ZRPOPCode.PlayerWon);

export const useGameSummary = defineStore('game-summary', () => {
  const summary = ref<GameSummaryEntry[]>([]);
  const dispatchEvent = useGameEventDispatch();

  const _receiveMessage: typeof summaryWatcher['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.PlayerWon) {
      summary.value = msg.data.summary
        .map(e => ({
          id: e.id,
          username: e.username,
          position: e.position,
          score: e.score
        }))
        .sort((a, b) => a.score - b.score);
    }
  };

  const playAgain = () => {
    dispatchEvent(ZRPOPCode._ResetState, {});
    RouterService.getRouter().replace('/game/wait');
  };

  const leave = () => {
    dispatchEvent(ZRPOPCode.LeaveGame, {});
    RouterService.getRouter().replace('/home');
  };

  const reset = () => {
    summary.value = [];
  };

  summaryWatcher.onMessage(_receiveMessage);
  summaryWatcher.onReset(reset);
  summaryWatcher.onClose(reset);

  return {
    summary,
    playAgain,
    leave,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
