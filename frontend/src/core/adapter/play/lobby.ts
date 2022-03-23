import { MonolithicEventWatcher } from '@/core/adapter/play/util/MonolithicEventWatcher';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import { ZRPJoinedGamePayload, ZRPLeftGamePayload, ZRPOPCode, ZRPRole } from '@/core/services/zrp/zrpTypes';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useGameEventDispatch } from '@/composables/eventDispatch';

export type LobbyPlayer = {
  id: string;
  name: string;
  role: ZRPRole;
};

const lobbyWatcher = new MonolithicEventWatcher(ZRPOPCode.PlayerJoined, ZRPOPCode.PlayerLeft, ZRPOPCode.SpectatorJoined, ZRPOPCode.SpectatorLeft);

export const useLobbyStore = defineStore('game-lobby', () => {
  const players = ref<LobbyPlayer[]>([
    {
      name: 'test',
      id: 'xxx',
      role: ZRPRole.Host
    }
  ]);
  const spectators = ref<LobbyPlayer[]>([]);
  const dispatchEvent = useGameEventDispatch();
  const snackbar = useSnackbar();
  const translations = useI18n();

  const _receiveMessage: typeof lobbyWatcher['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.PlayerJoined) {
      joinPlayer(msg.data, ZRPRole.Player);
    } else if (msg.code === ZRPOPCode.PlayerLeft) {
      leavePlayer(msg.data, ZRPRole.Player);
    } else if (msg.code === ZRPOPCode.SpectatorJoined) {
      joinPlayer(msg.data, ZRPRole.Spectator);
    } else if (msg.code === ZRPOPCode.SpectatorLeft) {
      leavePlayer(msg.data, ZRPRole.Spectator);
    }
  };

  const joinPlayer = (data: ZRPJoinedGamePayload, role: ZRPRole) => {
    if (role === ZRPRole.Spectator) {
      spectators.value.push({
        name: data.name,
        id: data.name,
        role: role
      });
    } else {
      players.value.push({
        name: data.name,
        id: data.name,
        role: role
      });
    }
    snackbar.pushMessage({
      message: translations.t(`snackbar.lobby.${role === ZRPRole.Spectator ? 'spectator' : 'player'}Joined`, [data.name]),
      position: SnackBarPosition.TopRight
    });
  };

  const leavePlayer = (data: ZRPLeftGamePayload, role: ZRPRole) => {
    if (role === ZRPRole.Spectator) {
      console.log(data, role, [...spectators.value]);
      spectators.value = spectators.value.filter(s => s.id !== data.name);
      console.log('after', [...spectators.value]);
    } else {
      players.value = players.value.filter(p => p.id !== data.name);
    }
    snackbar.pushMessage({
      message: translations.t(`snackbar.lobby.${role === ZRPRole.Spectator ? 'spectator' : 'player'}Left`, [data.name]),
      position: SnackBarPosition.TopRight
    });
  };

  const reset = () => {
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

  lobbyWatcher.onMessage(_receiveMessage);
  lobbyWatcher.onClose(reset);

  return {
    players: players,
    spectators: spectators,
    kickPlayer,
    promotePlayer
  };
});
