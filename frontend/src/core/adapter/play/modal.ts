import { defineStore } from 'pinia';
import { Component } from 'vue';
import ColorSelectModal from '@/components/game/modals/ColorSelectModal.vue';

export enum InGameModal {
  ColorPicker = 'ColorPicker'
}

const gameModals: Record<InGameModal, Component> = {
  [InGameModal.ColorPicker]: ColorSelectModal
};

export const useGameModal = defineStore('game-modal', {
  state: () => ({
    currentModal: undefined as string | undefined,
    modalComponent: undefined as Component | undefined
  }),

  actions: {
    openModal(type: InGameModal): void {
      this.currentModal = type;
      this.modalComponent = gameModals[type];
    },
    closeModal(): void {
      this.currentModal = undefined;
      this.modalComponent = undefined;
    }
  }
});
