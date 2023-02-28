import { defineStore } from 'pinia';

import { MonolithicEventWatcher } from '@/core/adapter/game/util/MonolithicEventWatcher';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import { ZRPOPCode, ZRPRole } from '@/core/domain/zrp/zrpTypes';

export type LobbyPlayer = {
  id: string;
  name: string;
  role: ZRPRole;
};

const errorWatcher = new MonolithicEventWatcher(
  ZRPOPCode._UnknownError,
  ZRPOPCode._ConnectionError,
  ZRPOPCode._ConnectionClosed,
  ZRPOPCode._ClientError,
  ZRPOPCode._DecodingError,
  ZRPOPCode._Connected,
  ZRPOPCode.GeneralError,
  ZRPOPCode.AccessDeniedError,
  ZRPOPCode.PlaceCardError,
  ZRPOPCode.LobbyFullError,
  ZRPOPCode.BotNameExistsError
);

export const useInGameErrorWatcher = defineStore('game-error-to-snackbar', () => {
  const snackbar = useSnackbar();

  const _receiveMessage: typeof errorWatcher['_msgHandler'] = msg => {
    snackbar.pushMessage({
      message: `errors.zrp.${msg.code}`,
      position: SnackBarPosition.Top,
      color: 'primary',
      force: true,
      needsTranslation: true
    });
  };

  errorWatcher.onMessage(_receiveMessage);

  return {
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
