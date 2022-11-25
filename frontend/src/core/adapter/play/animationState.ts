import { defineStore } from 'pinia';

export const useAnimationState = defineStore('game-animation-state', {
  state: () => ({
    mainCard: undefined as HTMLElement | undefined
  })
});
