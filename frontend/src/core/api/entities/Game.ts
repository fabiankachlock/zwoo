import { ZRPRole } from '@/core/domain/zrp/zrpTypes';

import { BackendErrorAble } from '../ApiError';

export type GameJoinResponse = BackendErrorAble<{
  id: number;
  isRunning: boolean;
  role: ZRPRole;
  ownId: number;
}>;

export type GameMeta = {
  id: number;
  name: string;
  isPublic: boolean;
  playerCount: number;
};

export type GamesList = GameMeta[];
