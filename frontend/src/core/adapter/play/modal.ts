import { defineStore } from 'pinia';
import { Component, markRaw, ref, watch } from 'vue';

import ColorSelectModal from '@/components/game/modals/ColorSelectModal.vue';
import { useGameEventDispatch } from '@/composables/useGameEventDispatch';
import { CardColor } from '@/core/services/game/card';
import { Awaiter } from '@/core/services/helper/Awaiter';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';

import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

export enum InGameModal {
  ColorPicker = 1
}

const gameModals: Record<InGameModal, Component> = {
  [InGameModal.ColorPicker]: ColorSelectModal
};

export type InGameModalResponse = {
  [InGameModal.ColorPicker]: CardColor;
};

const modalWatcher = new MonolithicEventWatcher(ZRPOPCode.GetPlayerDecision);

export const useGameModal = defineStore('game-modal', () => {
  const currentModal = ref<number | undefined>(undefined);
  const _modalResponse = ref<undefined | InGameModalResponse[keyof InGameModalResponse]>(undefined);
  const modalComponent = ref<Component | undefined>(undefined);
  const dispatchEvent = useGameEventDispatch();

  const openModal = (type: InGameModal): void => {
    currentModal.value = type;
    modalComponent.value = markRaw(gameModals[type]);
    _modalResponse.value = undefined; // reset response value
  };

  const closeSelf = (response: InGameModalResponse[keyof InGameModalResponse] | undefined) => {
    currentModal.value = undefined;
    modalComponent.value = undefined;
    _modalResponse.value = response;
  };

  const useResponse = async <ModalType extends InGameModal>(modal: ModalType): Promise<InGameModalResponse[ModalType]> => {
    const awaiter = new Awaiter<InGameModalResponse[ModalType]>();
    openModal(modal);
    watch(_modalResponse, response => {
      if (response) {
        awaiter.callback(response);
      }
    });
    return awaiter.promise;
  };

  modalWatcher.onMessage(async msg => {
    if (msg.code === ZRPOPCode.GetPlayerDecision) {
      const response = await useResponse(msg.data.type);
      dispatchEvent(ZRPOPCode.SendPlayerDecision, {
        decision: response as number,
        type: msg.data.type
      });
    }
  });

  const reset = () => {
    currentModal.value = undefined;
    modalComponent.value = undefined;
    _modalResponse.value = undefined;
  };

  modalWatcher.onReset(reset);
  modalWatcher.onClose(reset);

  return {
    currentModal,
    modalComponent,
    openModal,
    closeSelf,
    useResponse
  };
});
