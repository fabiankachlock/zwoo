import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

import { useGameEventDispatch } from '@/composables/useGameEventDispatch';
import { MonolithicEventWatcher } from '@/core/adapter/play/util/MonolithicEventWatcher';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import { arrayDiff } from '@/core/services/utils';
import {
  ZRPAllLobbyPlayersPayload,
  ZRPJoinedGamePayload,
  ZRPLeftGamePayload,
  ZRPNamePayload,
  ZRPOPCode,
  ZRPPlayerWithRolePayload,
  ZRPRole
} from '@/core/services/zrp/zrpTypes';
import { I18nInstance } from '@/i18n';
import router from '@/router';

import { useAuth } from '../auth';
import { useGameConfig } from '../game';
import { usePlayerManager } from './playerManager';

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
  ZRPOPCode.PromoteToHost,
  ZRPOPCode.GameStarted,
  ZRPOPCode.PlayerDisconnected,
  ZRPOPCode.PlayerReconnected
);

export const useLobbyStore = defineStore('game-lobby', () => {
  const playerManager = usePlayerManager();
  const players = computed<LobbyPlayer[]>(() => playerManager.players);
  const spectators = computed<LobbyPlayer[]>(() => playerManager.spectators);
  const gameHost = ref('');
  const dispatchEvent = useGameEventDispatch();
  const snackbar = useSnackbar();
  const translations = I18nInstance;
  const auth = useAuth();
  const gameConfig = useGameConfig();

  const _receiveMessage: typeof lobbyWatcher['_msgHandler'] = msg => {
    if (msg.code === ZRPOPCode.PlayerJoined) {
      joinPlayer(msg.data, ZRPRole.Player, true);
    } else if (msg.code === ZRPOPCode.PlayerLeft) {
      leavePlayer(msg.data, ZRPRole.Player, true);
    } else if (msg.code === ZRPOPCode.SpectatorJoined) {
      joinPlayer(msg.data, ZRPRole.Spectator, true);
    } else if (msg.code === ZRPOPCode.SpectatorLeft) {
      leavePlayer(msg.data, ZRPRole.Spectator, true);
    } else if (msg.code === ZRPOPCode.ListAllPlayers) {
      updatePlayers(msg.data);
    } else if (msg.code === ZRPOPCode.NewHost) {
      newHost(msg.data, true);
    } else if (msg.code === ZRPOPCode.PlayerChangedRole) {
      changePlayerRole(msg.data, true);
    } else if (msg.code === ZRPOPCode.PromoteToHost) {
      selfGotHost();
    } else if (msg.code == ZRPOPCode.GameStarted) {
      router.replace('/game/play');
    } else if (msg.code === ZRPOPCode.PlayerDisconnected) {
      playerManager.setPlayerDisconnected(msg.data.username);
    } else if (msg.code === ZRPOPCode.PlayerReconnected) {
      playerManager.setPlayerConnected(msg.data.username);
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
  };

  const joinPlayer = (data: ZRPJoinedGamePayload, role: ZRPRole, pushMessage = false) => {
    if (role === ZRPRole.Spectator) {
      playerManager.addPlayer({
        username: data.username,
        id: data.username,
        role: role
      });
    } else {
      playerManager.addPlayer({
        username: data.username,
        id: data.username,
        role: role
      });
    }
    if (pushMessage && data.username !== auth.username) {
      snackbar.pushMessage({
        message: translations.t(`snackbar.lobby.${role === ZRPRole.Spectator ? 'spectator' : 'player'}Joined`, [data.username]),
        position: SnackBarPosition.Top
      });
    }
  };

  const leavePlayer = (data: ZRPLeftGamePayload, role: ZRPRole, pushMessage = false) => {
    if (role === ZRPRole.Spectator) {
      playerManager.removePlayer(data.username);
    } else {
      playerManager.removePlayer(data.username);
    }
    if (pushMessage && data.username !== auth.username) {
      snackbar.pushMessage({
        message: translations.t(`snackbar.lobby.${role === ZRPRole.Spectator ? 'spectator' : 'player'}Left`, [data.username]),
        position: SnackBarPosition.Top
      });
    }
  };

  const newHost = (data: ZRPNamePayload, pushMessage = false) => {
    gameHost.value = data.username;
    if (pushMessage && data.username !== auth.username) {
      snackbar.pushMessage({
        message: translations.t(`snackbar.lobby.changedRoleToHost`, [data.username]),
        position: SnackBarPosition.Top
      });
    }
  };

  const changePlayerRole = (data: ZRPPlayerWithRolePayload, pushMessage = false) => {
    const player = players.value.find(player => player.id === data.username);
    const spectator = spectators.value.find(player => player.id === data.username);
    const user = player ?? spectator;

    if (!user) return; // no user existing
    if (data.role === ZRPRole.Player) {
      // changed to player
      playerManager.removePlayer(data.username);
      playerManager.addPlayer({
        username: data.username,
        id: data.username,
        role: data.role
      });
      if (pushMessage && data.username !== auth.username) {
        snackbar.pushMessage({
          message: translations.t(`snackbar.lobby.changedRoleToPlayer`, [data.username]),
          position: SnackBarPosition.Top
        });
      }
    } else if (data.role === ZRPRole.Spectator) {
      // changed to spectator
      playerManager.removePlayer(data.username);
      playerManager.addPlayer({
        username: data.username,
        id: data.username,
        role: data.role
      });
      if (pushMessage && data.username !== auth.username) {
        snackbar.pushMessage({
          message: translations.t(`snackbar.lobby.changedRoleToSpectator`, [data.username]),
          position: SnackBarPosition.Top
        });
      }
    }
    if (data.username === auth.username) {
      gameConfig.changeRole(data.role);
    }
  };

  const setup = () => {
    dispatchEvent(ZRPOPCode.GetAllPlayers, {});
  };

  const reset = () => {
    playerManager.reset();
    gameHost.value = '';
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

  const leaveSelf = () => {
    gameConfig.leave();
  };

  const startGame = () => {
    dispatchEvent(ZRPOPCode.StartGame, {});
  };

  const selfGotHost = () => {
    gameHost.value = auth.username;
    gameConfig.changeRole(ZRPRole.Host);
    snackbar.pushMessage({
      message: translations.t('snackbar.lobby.selfGotHost'),
      position: SnackBarPosition.Top
    });
  };

  lobbyWatcher.onOpen(setup);
  lobbyWatcher.onMessage(_receiveMessage);
  lobbyWatcher.onReset(() => {
    reset();
    setup();
  });
  lobbyWatcher.onClose(() => {
    reset();
    router.replace('/available-games');
  });

  return {
    players: players,
    spectators: spectators,
    host: gameHost,
    leave: leaveSelf,
    kickPlayer,
    promotePlayer,
    changeToSpectator,
    changeToPlayer,
    startGame,
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    __init__: () => {}
  };
});
