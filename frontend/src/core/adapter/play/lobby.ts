import { MonolithicEventWatcher } from '@/core/adapter/play/util/MonolithicEventWatcher';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import {
  ZRPAllLobbyPlayersPayload,
  ZRPJoinedGamePayload,
  ZRPLeftGamePayload,
  ZRPOPCode,
  ZRPPlayerWithRolePayload,
  ZRPRole,
  ZRPNamePayload
} from '@/core/services/zrp/zrpTypes';
import { defineStore } from 'pinia';
import { Ref, ref } from 'vue';
import { useGameEventDispatch } from '@/composables/eventDispatch';
import { arrayDiff, uniqueBy } from '@/core/services/utils';
import { I18nInstance } from '@/i18n';
import { useAuth } from '../auth';
import { useGameConfig } from '../game';

export type LobbyPlayer = {
  id: string;
  username: string;
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
  const players = ref<LobbyPlayer[]>([]);
  const spectators = ref<LobbyPlayer[]>([]);
  const gameHost = ref('');
  const dispatchEvent = useGameEventDispatch();
  const snackbar = useSnackbar();
  const translations = I18nInstance;
  const auth = useAuth();
  const gameConfig = useGameConfig();
  let isInitialFetch = false;

  const addPlayer = (list: Ref<LobbyPlayer[]>, player: LobbyPlayer) => {
    list.value = uniqueBy([...players.value, player], p => p.id);
  };

  const removePlayer = (list: Ref<LobbyPlayer[]>, id: string) => {
    list.value = list.value.filter(p => p.id !== id);
  };

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
        id: p.username
      }));
    const newSpectators = data.players
      .filter(p => p.role === ZRPRole.Spectator)
      .map(p => ({
        ...p,
        id: p.username
      }));

    for (const player of data.players) {
      if (player.role === ZRPRole.Host) {
        gameHost.value = player.username;
        break;
      }
    }

    const playerDiff = arrayDiff(players.value, newPlayers, (a, b) => a.id === b.id);
    const spectatorDiff = arrayDiff(spectators.value, newSpectators, (a, b) => a.id === b.id);

    for (const deletion of [...playerDiff.removed, ...spectatorDiff.removed]) {
      leavePlayer(deletion as unknown as ZRPJoinedGamePayload, deletion.role);
    }
    for (const addition of [...playerDiff.added, ...spectatorDiff.added]) {
      joinPlayer(addition as unknown as ZRPJoinedGamePayload, addition.role);
    }
    isInitialFetch = false;
  };

  const joinPlayer = (data: ZRPJoinedGamePayload, role: ZRPRole) => {
    if (role === ZRPRole.Spectator) {
      addPlayer(spectators, {
        username: data.username,
        id: data.username,
        role: role
      });
    } else {
      addPlayer(players, {
        username: data.username,
        id: data.username,
        role: role
      });
    }
    if (!isInitialFetch) {
      snackbar.pushMessage({
        message: translations.t(`snackbar.lobby.${role === ZRPRole.Spectator ? 'spectator' : 'player'}Joined`, [data.username]),
        position: SnackBarPosition.TopRight
      });
    }
  };

  const leavePlayer = (data: ZRPLeftGamePayload, role: ZRPRole) => {
    if (role === ZRPRole.Spectator) {
      removePlayer(spectators, data.username);
    } else {
      removePlayer(players, data.username);
    }
    if (!isInitialFetch) {
      snackbar.pushMessage({
        message: translations.t(`snackbar.lobby.${role === ZRPRole.Spectator ? 'spectator' : 'player'}Left`, [data.username]),
        position: SnackBarPosition.TopRight
      });
    }
  };

  const newHost = (data: ZRPNamePayload) => {
    gameHost.value = data.username;
  };

  const changePlayerRole = (data: ZRPPlayerWithRolePayload) => {
    const player = players.value.find(player => player.id === data.username);
    const spectator = spectators.value.find(player => player.id === data.username);
    const user = player ?? spectator;

    if (!user) return; // no user existing
    if (data.role === ZRPRole.Player) {
      removePlayer(spectators, data.username);
      addPlayer(players, {
        username: data.username,
        id: data.username,
        role: data.role
      });
    } else if (data.role === ZRPRole.Spectator) {
      removePlayer(players, data.username);
      addPlayer(spectators, {
        username: data.username,
        id: data.username,
        role: data.role
      });
    }
    if (data.username === auth.username) {
      gameConfig.changeRole(data.role);
    }

    snackbar.pushMessage({
      message: translations.t(`snackbar.lobby.${data.role === ZRPRole.Player ? 'spectator' : 'player'}ChangedRole`, [data.username]),
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

  const changeToSpectator = (id: string) => {
    dispatchEvent(ZRPOPCode.PlayerWantsToSpectate, { username: id });
  };

  const changeToPlayer = () => {
    dispatchEvent(ZRPOPCode.SpectatorWantsToPlay, {});
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
    changeToSpectator,
    changeToPlayer,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
