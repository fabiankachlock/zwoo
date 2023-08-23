import { defineStore } from 'pinia';
import { ref } from 'vue';

import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { ZRPOPCode, ZRPRole } from '@/core/domain/zrp/zrpTypes';
import { RouterService } from '@/core/global/Router';

import { usePlayerManager } from './playerManager';
import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export type GameSummaryEntry = {
  id: number;
  username: string;
  score: number;
  position: number;
  isBot: boolean;
};

const summaryWatcher = new MonolithicEventWatcher(ZRPOPCode.PlayerWon);

export const useGameSummary = defineStore('game-summary', () => {
  const summary = ref<GameSummaryEntry[]>([]);
  const dispatchEvent = useGameEventDispatch();
  const playerManager = usePlayerManager();

  const _receiveMessage: (typeof summaryWatcher)['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.PlayerWon) {
      summary.value = msg.data.summary
        .map(e => ({
          id: e.id,
          username: playerManager.getPlayerName(e.id),
          position: e.position,
          score: e.score,
          isBot: playerManager.getPlayerRole(e.id) === ZRPRole.Bot
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
