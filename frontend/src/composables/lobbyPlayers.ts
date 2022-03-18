import { useWatchGameEvents } from '@/core/adapter/play/util/gameEventWatcher';
import { createZRPOPCodeMatcher } from '@/core/adapter/play/util/zrpMatcher';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';
import { computed, ref } from 'vue';
import { useGameEventDispatch } from './eventDisptach';

export type LobbyPlayer = {
  id: string;
  name: string;
  role: string;
};

export const useLobbyPlayers = () => {
  const players = ref<LobbyPlayer[]>([
    {
      name: 'test',
      id: 'xxx',
      role: 'host'
    }
  ]);
  const spectators = ref<LobbyPlayer[]>([]);
  const dispatchEvent = useGameEventDispatch();

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

  const kickPlayer = (id: string) => {
    console.log('kick', id);
    dispatchEvent(ZRPOPCode.KickPlayer, {}); // TODO: fix payload
  };

  const promotePlayer = (id: string) => {
    console.log('kick', id);
    dispatchEvent(ZRPOPCode.PromotePlayerToHost, {}); // TODO: fix payload
  };

  return {
    players: players,
    spectators: spectators,
    host: computed(() => players.value.find(p => p.role === '')), // TODO: missing roles
    kickPlayer,
    promotePlayer,
    reset
  };
};
