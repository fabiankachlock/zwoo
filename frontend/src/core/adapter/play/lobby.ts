import { MonolithicEventWatcher } from '@/core/adapter/play/util/MonolithicEventWatcher';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import {
  ZRPAllLobbyPlayersPayload,
  ZRPJoinedGamePayload,
  ZRPLeftGamePayload,
  ZRPOPCode,
  ZRPPlayerWithRolePayload,
  ZRPRole,
  ZRPUsernamePayload
} from '@/core/services/zrp/zrpTypes';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { useGameEventDispatch } from '@/composables/eventDispatch';
import { arrayDiff } from '@/core/services/utils';
import { I18nInstance } from '@/i18n';
import { useAuth } from '../auth';
import { useGameConfig } from '../game';

export type LobbyPlayer = {
  id: string;
  name: string;
  role: ZRPRole;
};

const lobbyWatcher = new MonolithicEventWatcher(
  ZRPOPCode.PlayerJoined,
  ZRPOPCode.PlayerLeft,
  ZRPOPCode.SpectatorJoined,
  ZRPOPCode.SpectatorLeft,
  ZRPOPCode.ListAllPlayers,
  ZRPOPCode.NewHost,
  ZRPOPCode.PlayerChangedRole,
  ZRPOPCode.PromoteToHost
);

export const useLobbyStore = defineStore('game-lobby', () => {
  const players = ref<LobbyPlayer[]>([
    {
      id: '',
      name: 'a',
      role: ZRPRole.Host
    }
  ]);
  const spectators = ref<LobbyPlayer[]>([]);
  const gameHost = ref('');
  const dispatchEvent = useGameEventDispatch();
  const snackbar = useSnackbar();
  const translations = I18nInstance;
  const auth = useAuth();
  const gameConfig = useGameConfig();

  const _receiveMessage: typeof lobbyWatcher['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.PlayerJoined) {
      joinPlayer(msg.data, ZRPRole.Player);
    } else if (msg.code === ZRPOPCode.PlayerLeft) {
      leavePlayer(msg.data, ZRPRole.Player);
    } else if (msg.code === ZRPOPCode.SpectatorJoined) {
      joinPlayer(msg.data, ZRPRole.Spectator);
    } else if (msg.code === ZRPOPCode.SpectatorLeft) {
      leavePlayer(msg.data, ZRPRole.Spectator);
    } else if (msg.code === ZRPOPCode.ListAllPlayers) {
      updatePlayers(msg.data);
    } else if (msg.code === ZRPOPCode.NewHost) {
      newHost(msg.data);
    } else if (msg.code === ZRPOPCode.PlayerChangedRole) {
      changePlayerRole(msg.data);
    } else if (msg.code === ZRPOPCode.PromoteToHost) {
      gameHost.value = auth.username;
      gameConfig.changeRole(ZRPRole.Host);
    }
  };

  const updatePlayers = (data: ZRPAllLobbyPlayersPayload) => {
    const newPlayers = data.players
      .filter(p => p.role === ZRPRole.Player || p.role === ZRPRole.Host)
      .map(p => ({
        ...p,
        id: p.name
      }));
    const newSpectators = data.players
      .filter(p => p.role === ZRPRole.Spectator)
      .map(p => ({
        ...p,
        id: p.name
      }));

    const playerDiff = arrayDiff(players.value, newPlayers, (a, b) => a.id === b.id);
    const spectatorDiff = arrayDiff(spectators.value, newSpectators, (a, b) => a.id === b.id);

    for (const deletion of [...playerDiff.removed, ...spectatorDiff.removed]) {
      leavePlayer(deletion as unknown as ZRPJoinedGamePayload, deletion.role);
    }
    for (const addition of [...playerDiff.added, ...spectatorDiff.added]) {
      joinPlayer(addition as unknown as ZRPJoinedGamePayload, addition.role);
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

  const newHost = (data: ZRPUsernamePayload) => {
    gameHost.value = data.username;
  };

  const changePlayerRole = (data: ZRPPlayerWithRolePayload) => {
    const player = players.value.find(player => player.id === data.name);
    const spectator = spectators.value.find(player => player.id === data.name);
    const user = player ?? spectator;

    if (!user) return; // no user existing
    if (data.role === ZRPRole.Player) {
      spectators.value = spectators.value.filter(player => player.id === data.name);
      players.value.push({
        id: data.name,
        name: data.name,
        role: data.role
      });
    } else if (data.role === ZRPRole.Spectator) {
      players.value = players.value.filter(player => player.id === data.name);
      spectators.value.push({
        id: data.name,
        name: data.name,
        role: data.role
      });
    }
    if (data.name === auth.username) {
      gameConfig.changeRole(data.role);
    }

    snackbar.pushMessage({
      message: translations.t(`snackbar.lobby.${data.role === ZRPRole.Player ? 'spectator' : 'player'}ChangedRole`, [data.name]),
      position: SnackBarPosition.TopRight
    });
  };

  const setup = () => {
    dispatchEvent(ZRPOPCode.GetAllPlayers, {});
  };

  const reset = () => {
    //players.value = [];
  };

  const kickPlayer = (id: string) => {
    dispatchEvent(ZRPOPCode.KickPlayer, { username: id });
  };

  const promotePlayer = (id: string) => {
    dispatchEvent(ZRPOPCode.PromotePlayerToHost, { username: id });
  };

  lobbyWatcher.onMessage(_receiveMessage);
  lobbyWatcher.onClose(reset);
  lobbyWatcher.onOpen(setup);

  return {
    players: players,
    spectators: spectators,
    host: gameHost,
    kickPlayer,
    promotePlayer,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
