import { defineStore } from 'pinia';

export const useGameState = defineStore('game-state', {
  state: () => ({
    activePlayer: false
  }),
  actions: {
    setIsActive(active: boolean) {
      this.activePlayer = active;
    }
  }
});
