import { defineStore } from 'pinia';
import { Component, markRaw, ref, watch } from 'vue';

import ColorSelectModal from '@/components/game/modals/ColorSelectModal.vue';
import PlayerSelectModal from '@/components/game/modals/PlayerSelectModal.vue';
import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { ZRPDecisionType, ZRPOPCode } from '@/core/domain/zrp/zrpTypes';
import { Awaiter } from '@/core/helper/Awaiter';

import { MonolithicEventWatcher } from './util/MonolithicEventWatcher';

const gameModals: Record<ZRPDecisionType, Component> = {
  [ZRPDecisionType.ColorPicker]: ColorSelectModal,
  [ZRPDecisionType.PlayerSelector]: PlayerSelectModal
};

const modalWatcher = new MonolithicEventWatcher(ZRPOPCode.GetPlayerDecision);

export const useGameModal = defineStore('game-modal', () => {
  const currentModal = ref<number | undefined>(undefined);
  const currentOptions = ref<string[]>([]);
  const _modalResponse = ref<number | undefined>(undefined);
  const modalComponent = ref<Component | undefined>(undefined);
  const dispatchEvent = useGameEventDispatch();

  const openModal = (type: ZRPDecisionType, options: string[]): void => {
    currentModal.value = type;
    modalComponent.value = markRaw(gameModals[type]);
    _modalResponse.value = undefined; // reset response value
    currentOptions.value = options;
  };

  const closeSelf = (response: number | undefined) => {
    currentModal.value = undefined;
    modalComponent.value = undefined;
    _modalResponse.value = response;
    currentOptions.value = [];
  };

  const useResponse = async <ModalType extends ZRPDecisionType>(modal: ModalType, options: string[]): Promise<number | undefined> => {
    const awaiter = new Awaiter<number | undefined>();
    openModal(modal, options);
    watch(_modalResponse, response => {
      if (response !== undefined) {
        awaiter.callback(response);
      }
    });
    return awaiter.promise;
  };

  modalWatcher.onMessage(async msg => {
    if (msg.code === ZRPOPCode.GetPlayerDecision) {
      const response = await useResponse(msg.data.type, msg.data.options);
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
    currentOptions,
    modalComponent,
    openModal,
    closeSelf,
    useResponse,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
