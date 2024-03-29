import { useGameConfig } from '@/core/adapter/game';
import { ZRPOPCode, ZRPPayload } from '@/core/domain/zrp/zrpTypes';

export const useGameEventDispatch =
  () =>
  <C extends ZRPOPCode>(code: C, payload: ZRPPayload<C>) =>
    useGameConfig().sendEvent(code, payload);
