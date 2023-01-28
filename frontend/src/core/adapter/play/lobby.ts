import { defineStore } from 'pinia';
import { computed, ref } from 'vue';

import { MonolithicEventWatcher } from '@/core/adapter/play/util/MonolithicEventWatcher';
import { useGameEventDispatch } from '@/core/adapter/play/util/useGameEventDispatch';
import { SnackBarPosition, useSnackbar } from '@/core/adapter/snackbar';
import { RouterService } from '@/core/services/global/Router';
import { arrayDiff } from '@/core/services/utils';
import { ZRPAllLobbyPlayersPayload, ZRPIdPayload, ZRPNamePayload, ZRPOPCode, ZRPPlayerWithRolePayload, ZRPRole } from '@/core/services/zrp/zrpTypes';
import { I18nInstance } from '@/i18n';

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
  ZRPOPCode.BotJoined,
  ZRPOPCode.BotLeft,
  ZRPOPCode.ListAllPlayers,
  ZRPOPCode.NewHost,
  ZRPOPCode.PlayerChangedRole,
  ZRPOPCode.PromotedToHost,
  ZRPOPCode.GameStarted,
  ZRPOPCode.PlayerDisconnected,
  ZRPOPCode.PlayerReconnected
);

export const useLobbyStore = defineStore('game-lobby', () => {
  const playerManager = usePlayerManager();
  const players = computed<LobbyPlayer[]>(() => playerManager.players);
  const spectators = computed<LobbyPlayer[]>(() => playerManager.spectators);
  const bots = computed<LobbyPlayer[]>(() => playerManager.bots);
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
    } else if (msg.code === ZRPOPCode.BotJoined) {
      joinPlayer(msg.data, ZRPRole.Bot, true);
    } else if (msg.code === ZRPOPCode.BotLeft) {
      leavePlayer(msg.data, ZRPRole.Bot, true);
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
    } else if (msg.code === ZRPOPCode.PromotedToHost) {
      selfGotHost();
    } else if (msg.code == ZRPOPCode.GameStarted) {
      RouterService.getRouter().replace('/game/play');
    } else if (msg.code === ZRPOPCode.PlayerDisconnected) {
      playerManager.setPlayerDisconnected(msg.data.id);
    } else if (msg.code === ZRPOPCode.PlayerReconnected) {
      playerManager.setPlayerConnected(msg.data.id);
    }
  };

  const updatePlayers = (data: ZRPAllLobbyPlayersPayload) => {
    const newPlayers = data.players.filter(p => p.role === ZRPRole.Player || p.role === ZRPRole.Host || p.role === ZRPRole.Bot);
    const newSpectators = data.players.filter(p => p.role === ZRPRole.Spectator);

    for (const player of data.players) {
      if (player.role === ZRPRole.Host) {
        gameHost.value = player.id;
        break;
      }
    }

    const playerDiff = arrayDiff(players.value, newPlayers, (a, b) => a.id === b.id);
    const spectatorDiff = arrayDiff(spectators.value, newSpectators, (a, b) => a.id === b.id);

    for (const deletion of [...playerDiff.removed, ...spectatorDiff.removed]) {
      leavePlayer(deletion as unknown as ZRPNamePayload, deletion.role);
    }
    for (const addition of [...playerDiff.added, ...spectatorDiff.added]) {
      joinPlayer(addition as unknown as ZRPNamePayload, addition.role);
    }
  };

  const joinPlayer = (data: ZRPNamePayload, role: ZRPRole, pushMessage = false) => {
    playerManager.addPlayer({
      username: data.username,
      id: data.id,
      role: role
    });

    if (pushMessage && data.username !== auth.username) {
      snackbar.pushMessage({
        message: translations.t(`snackbar.lobby.${role === ZRPRole.Spectator ? 'spectator' : 'player'}Joined`, [data.username]),
        position: SnackBarPosition.Top
      });
    }
  };

  const leavePlayer = (data: ZRPIdPayload, role: ZRPRole, pushMessage = false) => {
    const player = playerManager.getPlayer(data.id);
    if (!player) return;

    playerManager.removePlayer(data.id);
    if (pushMessage && player.username !== auth.username) {
      snackbar.pushMessage({
        message: translations.t(`snackbar.lobby.${role === ZRPRole.Spectator ? 'spectator' : 'player'}Left`, [player.username]),
        position: SnackBarPosition.Top
      });
    }
  };

  const newHost = (data: ZRPIdPayload, pushMessage = false) => {
    const player = playerManager.getPlayer(data.id);
    if (!player) return;

    gameHost.value = data.id;
    if (pushMessage && player.username !== auth.username) {
      snackbar.pushMessage({
        message: translations.t(`snackbar.lobby.changedRoleToHost`, [player.username]),
        position: SnackBarPosition.Top
      });
    }
  };

  const changePlayerRole = (data: ZRPPlayerWithRolePayload, pushMessage = false) => {
    const player = players.value.find(player => player.id === data.id);
    const spectator = spectators.value.find(player => player.id === data.id);

    if (data.role === ZRPRole.Player && spectator) {
      // changed to player
      playerManager.removePlayer(data.id);
      playerManager.addPlayer({
        username: spectator.username,
        id: data.id,
        role: data.role
      });
      if (pushMessage && data.id !== auth.publicId) {
        snackbar.pushMessage({
          message: translations.t(`snackbar.lobby.changedRoleToPlayer`, [spectator.username]),
          position: SnackBarPosition.Top
        });
      }
    } else if (data.role === ZRPRole.Spectator && player) {
      // changed to spectator
      playerManager.removePlayer(data.id);
      playerManager.addPlayer({
        username: player.username,
        id: data.id,
        role: data.role
      });
      if (pushMessage && data.id !== auth.publicId) {
        snackbar.pushMessage({
          message: translations.t(`snackbar.lobby.changedRoleToSpectator`, [player.username]),
          position: SnackBarPosition.Top
        });
      }
    }
    if (data.id === auth.publicId) {
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
    dispatchEvent(ZRPOPCode.KickPlayer, { id: id });
  };

  const promotePlayer = (id: string) => {
    dispatchEvent(ZRPOPCode.PromotePlayerToHost, { id: id });
  };

  const changeToSpectator = (id: string) => {
    dispatchEvent(ZRPOPCode.PlayerToSpectator, { id: id });
  };

  const changeToPlayer = () => {
    dispatchEvent(ZRPOPCode.SpectatorToPlayer, {});
  };

  const leaveSelf = () => {
    gameConfig.leave();
  };

  const startGame = () => {
    dispatchEvent(ZRPOPCode.StartGame, {});
  };

  const selfGotHost = () => {
    gameHost.value = auth.publicId;
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
    RouterService.getRouter().replace('/available-games');
  });

  return {
    bots: bots,
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
