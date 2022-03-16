import { defineStore } from 'pinia';
import { ref } from 'vue';

export enum SnackBarPosition {
  Top,
  TopLeft,
  TopRight,
  Left,
  Right,
  Bottom,
  BottomLeft,
  BottomRight
}

export const SnackBarPositionClasses: Record<SnackBarPosition, string> = {
  [SnackBarPosition.Top]: 'top-5 right-5 left-5 flex justify-center',
  [SnackBarPosition.TopLeft]: 'top-5 left-5',
  [SnackBarPosition.TopRight]: 'top-5 right-5',
  [SnackBarPosition.Left]: 'left-5 top-5 bottom-5 flex items-center',
  [SnackBarPosition.Right]: 'right-5 top-5 bottom-5 flex items-center',
  [SnackBarPosition.Bottom]: 'bottom-5 right-5 left-5 flex justify-center',
  [SnackBarPosition.BottomLeft]: 'bottom-5 left-5',
  [SnackBarPosition.BottomRight]: 'bottom-5 right-5'
};

export type SnackbarItem = {
  message: string;
  needsTranslation?: boolean;
  duration?: number;
  force?: boolean;
  position: SnackBarPosition;
};

const DEFAULT_DURATION = 1000;

export const useSnackbar = defineStore('snackbar', () => {
  const activeMessage = ref<SnackbarItem | undefined>(undefined);
  const activeTimeout = ref<number | undefined>(undefined);
  const messageStack = ref<SnackbarItem[]>([]);

  const pushMessage = (msg: SnackbarItem) => {
    console.log('push');
    if (activeMessage.value === undefined) {
      showMessage(msg);
      return;
    }
    // queue item
    if (msg.force === true) {
      if (activeTimeout.value) {
        clearTimeout(activeTimeout.value);
      }
      showMessage(msg);
      return;
    }
    messageStack.value.push(msg);
  };

  const showMessage = (msg: SnackbarItem) => {
    console.log('show', msg.message, msg.duration ?? DEFAULT_DURATION);
    activeMessage.value = msg;
    activeTimeout.value = setTimeout(() => {
      console.log('done');
      activeMessage.value = undefined;
      activeTimeout.value = undefined;
      evaluateNext();
    }, msg.duration ?? DEFAULT_DURATION);
  };

  const evaluateNext = () => {
    const msg = messageStack.value.shift();
    if (msg) {
      showMessage(msg);
    }
  };

  const close = () => {
    activeMessage.value = undefined;
  };

  return {
    activeMessage,
    pushMessage,
    close
  };
});
