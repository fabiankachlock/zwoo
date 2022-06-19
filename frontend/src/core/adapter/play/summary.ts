import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';
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

  const _receiveMessage: typeof summaryWatcher['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.PlayerWon) {
      summary.value = msg.data.summary.map(e => ({
        username: e.username,
        position: e.position,
        score: e.score
      }));
    }
  };

  const joinAgainAsPlayer = () => {
    console.log('joining as player');
  };

  const joinAgainAsSpectator = () => {
    console.log('joining again as spectator');
  };

  const leave = () => {
    console.log('leaving game');
  };

  summaryWatcher.onMessage(_receiveMessage);
  summaryWatcher.onClose(() => {
    summary.value = [];
  });

  return {
    summary,
    joinAgainAsPlayer,
    joinAgainAsSpectator,
    leave,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
