import { defineStore } from 'pinia';
import { Component, markRaw } from 'vue';
import ColorSelectModal from '@/components/game/modals/ColorSelectModal.vue';
import { CardColor } from '@/core/services/game/card';

export enum InGameModal {
  ColorPicker = 'ColorPicker'
}

const gameModals: Record<InGameModal, Component> = {
  [InGameModal.ColorPicker]: ColorSelectModal
};

export type InGameModalResponse = {
  [InGameModal.ColorPicker]: CardColor;
};

export const useGameModal = defineStore('game-modal', {
  state: () => ({
    currentModal: undefined as string | undefined,
    _modalResponse: undefined as undefined | InGameModalResponse[keyof InGameModalResponse],
    modalComponent: undefined as Component | undefined
  }),

  actions: {
    openModal(type: InGameModal): void {
      this.currentModal = type;
      this.modalComponent = markRaw(gameModals[type]);
    },
    closeSelf(response: InGameModalResponse[keyof InGameModalResponse] | undefined): void {
      this.currentModal = undefined;
      this.modalComponent = undefined;
      this._modalResponse = response;
    }
  }
});
