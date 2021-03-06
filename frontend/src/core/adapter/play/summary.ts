import { useGameEventDispatch } from '@/composables/eventDispatch';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';
import router from '@/router';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type GameSummaryEntry = {
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
      summary.value = msg.data.summary.map(e => ({
        username: e.username,
        position: e.position,
        score: e.score
      }));
    }
  };

  const playAgain = () => {
    dispatchEvent(ZRPOPCode._ResetState, {});
    router.replace('/game/wait');
  };

  const leave = () => {
    dispatchEvent(ZRPOPCode.LeaveGame, {});
    router.replace('/home');
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
