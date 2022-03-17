import { useWatchGameEvents } from '@/core/adapter/play/util/gameEventWatcher';
import { createZRPOPCodeMatcher } from '@/core/adapter/play/util/zrpMatcher';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';
import { computed, ref } from 'vue';

export type LobbyPlayer = {
  id: string;
  name: string;
  role: string;
};

export const useLobbyPlayers = () => {
  const players = ref<LobbyPlayer[]>([]);
  const spectators = ref<LobbyPlayer[]>([]);

  useWatchGameEvents<ZRPOPCode.PlayerJoined | ZRPOPCode.PlayerLeft>(createZRPOPCodeMatcher(ZRPOPCode.PlayerJoined, ZRPOPCode.PlayerLeft), msg => {
    if (msg.code === ZRPOPCode.PlayerJoined) {
      players.value.push({
        name: msg.data.name,
        id: msg.data.name,
        role: '' // TODO: missing: roles
      });
    } else if (msg.code === ZRPOPCode.PlayerLeft) {
      players.value = players.value.filter(p => p.id !== msg.data.name);
    }
  });

  const reset = () => {
    // TODO: try call get players
    players.value = [];
  };

  return {
    players: players,
    spectators: spectators,
    host: computed(() => players.value.filter(p => p.role === '')), // TODO: missing roles
    reset
  };
};
