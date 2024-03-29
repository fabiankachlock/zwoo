import { defineStore } from 'pinia';

import { useGameEventDispatch } from '@/core/adapter/game/util/useGameEventDispatch';
import { useWakeLock } from '@/core/adapter/helper/useWakeLock';
import { getBackendErrorTranslation } from '@/core/api/ApiError';
import { ZRPMessageBuilder } from '@/core/domain/zrp/zrpBuilder';
import { ZRPCoder } from '@/core/domain/zrp/zrpCoding';
import { ZRPWebsocketAdapter } from '@/core/domain/zrp/ZRPMessageDistributer';
import { ZRPCardPayload, ZRPOPCode, ZRPPayload, ZRPRole } from '@/core/domain/zrp/zrpTypes';
import { RouterService } from '@/core/global/Router';
import Logger from '@/core/services/logging/logImport';
import { GameNameValidator } from '@/core/services/validator/gameName';

import { GameMeta } from '../api/entities/Game';
import { useGameEvents } from './game/events';
import { useApi } from './helper/useApi';

export type SavedGame = {
  id: number;
  role: ZRPRole;
};

let initializedGameModules = false;
const lastGameKey = 'zwoo:lg';

export const useGameConfig = defineStore('game-config', {
  state: () => ({
    allGames: [] as GameMeta[],
    gameId: undefined as number | undefined,
    name: '',
    role: undefined as ZRPRole | undefined,
    lobbyId: undefined as number | undefined,
    inActiveGame: false,
    _connection: undefined as ZRPWebsocketAdapter | undefined,
    _wakeLock: () => {
      return;
    }
  }),
  getters: {
    // TODO: delete this?
    host: state => state.role === ZRPRole.Host
  },
  actions: {
    changeRole(newRole: ZRPRole | undefined) {
      this.role = newRole;
      if (newRole && this.gameId) {
        this._saveConfig({ id: this.gameId, role: newRole });
      }
    },
    async create(name: string, isPublic: boolean, password: string) {
      const nameValid = new GameNameValidator().validate(name);
      if (!nameValid.isValid) throw nameValid.getErrors();

      const status = await useApi().createGame(name, isPublic, isPublic ? '' : password);
      if (status.isError) {
        throw getBackendErrorTranslation(status.error);
      } else {
        this.$patch({
          inActiveGame: true,
          role: status.data.role,
          gameId: status.data.gameId,
          lobbyId: status.data.ownId,
          name: name
        });
        this.connect();
        this._saveConfig({ id: status.data.gameId, role: status.data.role });
      }
    },
    async join(id: number, password: string, asPlayer: boolean, asSpectator: boolean) {
      if (asPlayer && asSpectator) {
        throw new Error('cant join as player || spectator');
      }

      const data = await this.getGameMeta(id);

      const status = await useApi().joinGame(id, asPlayer ? ZRPRole.Player : ZRPRole.Spectator, password);
      if (status.isError) {
        throw getBackendErrorTranslation(status.error);
      } else {
        this.$patch({
          inActiveGame: true,
          role: status.data.role,
          lobbyId: status.data.ownId,
          gameId: status.data.gameId,
          name: data?.name ?? 'error'
        });
        this.connect(status.data.isRunning);
        this._saveConfig({ id: status.data.gameId, role: status.data.role });
      }
    },
    leave(keepSavedGame = false): void {
      if (this.inActiveGame) {
        useGameEventDispatch()(ZRPOPCode.LeaveGame, {});
        this._connection?.close();
        this._wakeLock(); // relief wakelock
        queueMicrotask(() => useGameEvents().clear());
        if (!keepSavedGame) {
          this.clearStoredConfig();
        }
        this.$patch({
          inActiveGame: false,
          gameId: undefined,
          name: '',
          role: undefined,
          _connection: undefined
        });
        RouterService.getRouter().replace('/available-games');
      }
    },
    async listGames(): Promise<GameMeta[]> {
      const response = await useApi().loadAvailableGames();
      if (response.wasSuccessful) {
        this.allGames = response.data.games;
        return response.data.games;
      }
      return [];
    },
    async getGameMeta(id: number): Promise<GameMeta | undefined> {
      const game = this.allGames.find(game => game.id === id);
      if (game) {
        return game;
      }

      const response = await useApi().loadGameMeta(id);
      return response.wasSuccessful ? response.data : undefined;
    },
    async _initGameModules(): Promise<void> {
      if (!initializedGameModules) {
        (await import('./game/util/errorToSnackbar')).useInGameErrorWatcher().__init__();
        (await import('./game/cardTheme')).useCardTheme().__init__();
        (await import('./game/chat')).useChatStore().__init__();
        (await import('./game/deck')).useGameCardDeck().__init__();
        (await import('./game/events')).useGameEvents().__init__();
        (await import('./game/gameState')).useGameState().__init__();
        (await import('./game/lobby')).useLobbyStore().__init__();
        (await import('./game/rules')).useRules().__init__();
        (await import('./game/summary')).useGameSummary().__init__();
        (await import('./game/botManager')).useBotManager().__init__();
        (await import('./game/modal')).useGameModal().__init__();
        (await import('./game/feedback')).useGameFeedback().__init__();
        (await import('./game/util/keepAlive')).useKeepAlive().__init__();
        await (await import('./game/features/gameProfiles/useGameProfiles')).useGameProfiles().__init__();
        (await import('./game/features/chatBroadcast')).useChatBroadcast().__init__();
        (await import('./game/features/feedback/consumer/feedbackChatAdapter')).useFeedbackChatAdapter().__init__();
        (await import('./game/features/feedback/consumer/feedbackSnackbarAdapter')).useFeedbackSnackbarAdapter().__init__();
        initializedGameModules = true;
      }
    },
    async connect(isRunning = false) {
      await this._initGameModules();
      setTimeout(() => {
        this._connection = new ZRPWebsocketAdapter(useApi().createConnection((this.gameId ?? -1).toString()));
        const events = useGameEvents();
        this._connection.readMessages(events.handleIncomingEvent);

        useWakeLock().then(lock => {
          if (lock) {
            this._wakeLock = lock;
          }
        });

        if (isRunning) {
          events.pushAfter(
            evt => evt.code === ZRPOPCode._Connected,
            ZRPMessageBuilder.build(ZRPOPCode.GameStarted, {
              hand: [],
              pile: undefined as unknown as ZRPCardPayload,
              players: []
            })
          );
        }
      }, 0);
    },
    async tryLeave() {
      Logger.RouterGuard.warn('routing out of active game');
      if (this._connection) {
        Logger.RouterGuard.warn('force closing game connection');
        this.leave(true);
      }
    },
    async sendEvent<C extends ZRPOPCode>(code: C, payload: ZRPPayload<C>) {
      if (this._connection) {
        Logger.Zrp.log(`[outgoing] ${code} ${JSON.stringify(payload)}`);
        if (!ZRPCoder.isInternalMessage(code)) {
          this._connection.writeMessage(ZRPMessageBuilder.build(code, payload));
        } else {
          useGameEvents().pushEvent(ZRPMessageBuilder.build(code, payload));
        }
      }
    },
    _saveConfig(config: SavedGame) {
      localStorage.setItem(lastGameKey, JSON.stringify(config));
    },
    async tryRestoreStoredConfig(): Promise<(GameMeta & SavedGame) | undefined> {
      const storedConfig = localStorage.getItem(lastGameKey);
      if (storedConfig) {
        try {
          const config = JSON.parse(storedConfig) as SavedGame;
          const meta = await this.getGameMeta(config.id);
          if (meta)
            return {
              ...config,
              ...meta
            };
          this.clearStoredConfig();
        } catch {
          return undefined;
        }
      }
      return undefined;
    },
    clearStoredConfig() {
      localStorage.removeItem(lastGameKey);
    }
  }
});
