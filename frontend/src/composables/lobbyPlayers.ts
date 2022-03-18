import { useWatchGameEvents } from '@/core/adapter/play/util/gameEventWatcher';
import { createZRPOPCodeMatcher } from '@/core/adapter/play/util/zrpMatcher';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import { ZRPOPCode } from '@/core/services/zrp/zrpTypes';
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';
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
  const snackbar = useSnackbar();
  const translations = useI18n();

  useWatchGameEvents<ZRPOPCode.PlayerJoined | ZRPOPCode.PlayerLeft | ZRPOPCode.SpectatorJoined | ZRPOPCode.SpectatorLeft>(
    createZRPOPCodeMatcher(ZRPOPCode.PlayerJoined, ZRPOPCode.PlayerLeft, ZRPOPCode.SpectatorJoined, ZRPOPCode.SpectatorLeft),
    msg => {
      if (msg.code === ZRPOPCode.PlayerJoined) {
        players.value.push({
          name: msg.data.name,
          id: msg.data.name,
          role: '' // TODO: missing: roles
        });
        snackbar.pushMessage({
          message: translations.t('snackbar.lobby.playerJoined', [msg.data.name]),
          position: SnackBarPosition.TopRight
        });
      } else if (msg.code === ZRPOPCode.PlayerLeft) {
        players.value = players.value.filter(p => p.id !== msg.data.name);
        snackbar.pushMessage({
          message: translations.t('snackbar.lobby.playerLeft', [msg.data.name]),
          position: SnackBarPosition.TopRight
        });
      } else if (msg.code === ZRPOPCode.SpectatorJoined) {
        spectators.value.push({
          name: msg.data.name,
          id: msg.data.name,
          role: '' // TODO: missing roles
        });
        snackbar.pushMessage({
          message: translations.t('snackbar.lobby.spectatorJoined', [msg.data.name]),
          position: SnackBarPosition.TopRight
        });
      } else if (msg.code === ZRPOPCode.SpectatorLeft) {
        spectators.value = spectators.value.filter(s => s.id !== msg.data.name);
        snackbar.pushMessage({
          message: translations.t('snackbar.lobby.spectatorLeft', [msg.data.name]),
          position: SnackBarPosition.TopRight
        });
      }
    }
  );

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
