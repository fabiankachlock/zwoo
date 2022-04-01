import { defineStore } from 'pinia';

export const useGameSummary = defineStore('game-summary', {
  state: () => ({
    positions: {} as Record<string, number>
  }),
  actions: {
    async requestGameSummary(): Promise<{ name: string; position: number; score: number }[]> {
      this._computePositions();
      return Object.entries(this.positions)
        .map(([name, position]) => ({
          name,
          position,
          score: Math.floor(Math.random() * 20)
        }))
        .sort((a, b) => a.position - b.position);
    },
    _computePositions() {
      this.positions['test-winner'] = 1;
      this.positions['test-second'] = 2;
      this.positions['test-third'] = 3;
      this.positions['test-third-2'] = 3;
    }
  }
});
