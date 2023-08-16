import { defineStore } from 'pinia';
import { ref } from 'vue';

import Logger from '../services/logging/logImport';

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
  [SnackBarPosition.Top]: 'top-5 right-5 left-5 flex justify-center text-center',
  [SnackBarPosition.TopLeft]: 'top-5 left-5 text-left',
  [SnackBarPosition.TopRight]: 'top-5 right-5 text-right',
  [SnackBarPosition.Left]: 'left-5 top-5 bottom-5 flex items-center text-left',
  [SnackBarPosition.Right]: 'right-5 top-5 bottom-5 flex items-center text-right',
  [SnackBarPosition.Bottom]: 'bottom-5 right-5 left-5 flex justify-center text-center',
  [SnackBarPosition.BottomLeft]: 'bottom-5 left-5 text-left',
  [SnackBarPosition.BottomRight]: 'bottom-5 right-5 text-right'
};

export type SnackbarItem = {
  message: string;
  needsTranslation?: boolean;
  duration?: number;
  force?: boolean;
  position: SnackBarPosition;
  showClose?: boolean;
  hideProgress?: boolean;
  color?: 'primary' | 'secondary';
  mode: 'static' | 'loading';
  onClosed: () => void;
};

export type SnackbarItemOptions = Omit<SnackbarItem, 'mode' | 'onClosed'> & Partial<Pick<SnackbarItem, 'mode' | 'onClosed'>>;

const DEFAULT_DURATION = 2500;

const normalizeOptions = (opts: SnackbarItemOptions): SnackbarItem => {
  return {
    mode: 'static',
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    onClosed: () => {},
    ...opts
  };
};

export const useSnackbar = defineStore('snackbar', () => {
  const activeMessage = ref<SnackbarItem | undefined>(undefined);
  const activeTimeout = ref<number | undefined>(undefined);
  const messageStack = ref<SnackbarItem[]>([]);
  const logger = Logger.createOne('snackbar');

  const pushMessage = (item: SnackbarItemOptions) => {
    logger.log('push one');
    const msg = normalizeOptions(item);
    if (activeMessage.value === undefined) {
      showMessage(msg);
      return;
    }
    // queue item
    if (msg.force === true || activeMessage.value?.mode === 'loading') {
      logger.log('forcing message');
      if (activeTimeout.value) {
        clearTimeout(activeTimeout.value);
      }

      activeMessage.value.onClosed();
      activeMessage.value = undefined;
      messageStack.value = messageStack.value.filter(msg => msg.mode === 'static'); // remove any loading snackbars when force pushing
      setTimeout(() => {
        // wait until the next event loop, so that the animation can be reset before
        showMessage(msg);
      }, 0);
      return;
    }
    messageStack.value.push(msg);
  };

  const showMessage = (msg: SnackbarItem) => {
    msg.duration = msg.duration ?? DEFAULT_DURATION;
    activeMessage.value = msg;
    if (msg.mode === 'static') {
      activeTimeout.value = setTimeout(() => {
        activeMessage.value?.onClosed();
        activeMessage.value = undefined;
        activeTimeout.value = undefined;
        evaluateNext();
      }, msg.duration);
    }
  };

  const evaluateNext = () => {
    // wait until the next event loop, so that the animation can be reset before
    setTimeout(() => {
      if (!activeMessage.value) {
        const msg = messageStack.value.shift();
        if (msg) {
          showMessage(msg);
        }
      }
    }, 0);
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
