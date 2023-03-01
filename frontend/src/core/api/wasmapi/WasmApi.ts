import { ZRPRole } from '@/core/domain/zrp/zrpTypes';
import { WasmManger } from '@/core/services/wasm/WasmManager';

import { ApiAdapter } from '../ApiAdapter';
import { GameAdapter } from '../GameAdapter';
import { NoopApi } from '../noopapi/NoopApi';
import { WasmSocket } from './WasmSocket';

export const WasmApi: ApiAdapter & GameAdapter = {
  ...NoopApi,
  createGame: async (name, isPublic) => {
    const instance = await WasmManger.global.getInstance();
    await instance.GameManager.CreateGame(name, isPublic);
    await instance.GameManager.AddPlayer('OfflinePlayer');
    return {
      id: 1,
      isRunning: false,
      role: ZRPRole.Host
    };
  },
  createConnection() {
    return new WasmSocket();
  }
};
