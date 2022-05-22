import { Card } from '@/core/services/game/card';
import { defineStore } from 'pinia';

export type GamePlayer = {
  name: string;
  cards: number;
};

export const useGameState = defineStore('game-state', {
  state: () => ({
    isActivePlayer: false,
    mainCard: {
      color: 1,
      type: 1
    } as Card | undefined,
    activePlayer: '',
    players: [
      {
        name: 'abc',
        cards: 5
      },
      {
        name: 'askfdasda',
        cards: 2
      },
      {
        name: 'askfda',
        cards: 2
      },
      {
        name: 'adkulgsada',
        cards: 2
      },
      {
        name: '893olads',
        cards: 2
      },
      {
        name: 'pq0zgduiasas',
        cards: 2
      },
      {
        name: 'adlasda',
        cards: 9
      }
    ] as GamePlayer[]
  }),
  actions: {
    setIsActive(active: boolean) {
      this.isActivePlayer = active;
    },
    async update() {
      this.activePlayer = this.players[0].name;
    },
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  }
});
