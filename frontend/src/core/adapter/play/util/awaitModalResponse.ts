import { Awaiter } from '@/core/services/helper/Awaiter';
import { watch } from 'vue';
import { InGameModalResponse, InGameModal, useGameModal } from '../modal';

export const useModalResponse = async <ModalType extends InGameModal>(modal: ModalType): Promise<InGameModalResponse[ModalType]> => {
  const modalState = useGameModal();
  modalState.openModal(modal);

  const awaiter = new Awaiter<InGameModalResponse[ModalType]>();
  watch(
    () => modalState._modalResponse,
    response => awaiter.callback(response)
  );
  return awaiter.promise;
};
