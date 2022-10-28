import { computed } from 'vue';

import { useGameConfig } from '@/core/adapter/game';
import { ZRPRole } from '@/core/services/zrp/zrpTypes';

export const useIsHost = () => {
  const gameConfig = useGameConfig();
  return {
    isHost: computed(() => gameConfig.role === ZRPRole.Host)
  };
};

export const useIsPlayer = () => {
  const gameConfig = useGameConfig();
  return {
    isPlayer: computed(() => gameConfig.role === ZRPRole.Player)
  };
};

export const useIsSpectator = () => {
  const gameConfig = useGameConfig();
  return {
    isSpectator: computed(() => gameConfig.role === ZRPRole.Spectator)
  };
};
