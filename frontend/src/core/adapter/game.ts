import { defineStore } from 'pinia';

import { useGameEventDispatch } from '@/composables/eventDispatch';
import { useWakeLock } from '@/composables/useWakeLock';
import router from '@/router';

import { Backend, Endpoint } from '../services/api/apiConfig';
import { getBackendErrorTranslation, unwrapBackendError } from '../services/api/errors';
import { GameManagementService, GameMeta, GamesList } from '../services/api/GameManagement';
import Logger from '../services/logging/logImport';
import { GameNameValidator } from '../services/validator/gameName';
import { ZRPWebsocketAdapter } from '../services/ws/MessageDistributer';
import { ZRPMessageBuilder } from '../services/zrp/zrpBuilder';
import { ZRPCoder } from '../services/zrp/zrpCoding';
import { ZRPOPCode, ZRPPayload, ZRPRole } from '../services/zrp/zrpTypes';
import { useGameEvents } from './play/events';

export type SavedGame = {
  id: number;
  role: ZRPRole;
};

let initializedGameModules = false;
const lastGameKey = 'zwoo:lg';

export const useGameConfig = defineStore('game-config', {
  state: () => ({
    allGames: [] as GamesList,
    gameId: undefined as number | undefined,
    name: '',
    role: undefined as ZRPRole | undefined,
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

      const status = await GameManagementService.createGame(name, isPublic, isPublic ? '' : password);
      const [game, error] = unwrapBackendError(status);
      if (error) {
        throw getBackendErrorTranslation(error);
      } else if (game) {
        this.$patch({
          inActiveGame: true,
          role: game.role,
          gameId: game.id,
          name: name
        });
        this.connect();
        this._saveConfig({ id: game.id, role: game.role });
      }
    },
    async join(id: number, password: string, asPlayer: boolean, asSpectator: boolean) {
      if (asPlayer && asSpectator) {
        throw new Error('cant join as player & spectator');
      }

      const data = await this.getGameMeta(id);

      const status = await GameManagementService.joinGame(id, asPlayer ? ZRPRole.Player : ZRPRole.Spectator, password);
      const [game, error] = unwrapBackendError(status);
      if (error) {
        throw getBackendErrorTranslation(error);
      } else if (game) {
        this.$patch({
          inActiveGame: true,
          role: game.role,
          gameId: game.id,
          name: data?.name ?? 'error'
        });
        this.connect(game.isRunning);
        this._saveConfig({ id: game.id, role: game.role });
      }
    },
    leave(): void {
      if (this.inActiveGame) {
        useGameEventDispatch()(ZRPOPCode.LeaveGame, {});
        this._connection?.close();
        this._wakeLock(); // relief wakelock
        useGameEvents().clear();
        this.clearStoredConfig();
        this.$patch({
          inActiveGame: false,
          gameId: undefined,
          name: '',
          role: undefined,
          _connection: undefined
        });
        router.replace('/available-games');
      }
    },
    async listGames(): Promise<GamesList> {
      const response = await GameManagementService.listAll();
      const [games, error] = unwrapBackendError(response);
      if (!error) {
        this.allGames = games;
        return games;
      }
      return [];
    },
    async getGameMeta(id: number): Promise<GameMeta | undefined> {
      const game = this.allGames.find(game => game.id === id);
      if (game) {
        return game;
      }

      const response = await GameManagementService.getJoinMeta(id);
      return unwrapBackendError(response)[0];
    },
    async _initGameModules(): Promise<void> {
      if (!initializedGameModules) {
        (await import(/* webpackChunkName: "game-logic" */ './play/util/errorToSnackbar')).useInGameErrorWatcher().__init__();
        (await import(/* webpackChunkName: "game-logic" */ './play/cardTheme')).useCardTheme().__init__();
        (await import(/* webpackChunkName: "game-logic" */ './play/chat')).useChatStore().__init__();
        (await import(/* webpackChunkName: "game-logic" */ './play/deck')).useGameCardDeck().__init__();
        (await import(/* webpackChunkName: "game-logic" */ './play/events')).useGameEvents().__init__();
        (await import(/* webpackChunkName: "game-logic" */ './play/gameState')).useGameState().__init__();
        (await import(/* webpackChunkName: "game-logic" */ './play/lobby')).useLobbyStore().__init__();
        (await import(/* webpackChunkName: "game-logic" */ './play/rules')).useRules().__init__();
        (await import(/* webpackChunkName: "game-logic" */ './play/summary')).useGameSummary().__init__();
        (await import(/* webpackChunkName: "game-logic" */ './play/util/keepAlive')).useKeepAlive().__init__();
        (await import(/* webpackChunkName: "internal" */ './play/features/chatBroadcast')).useChatBroadcast().__init__();
        initializedGameModules = true;
      }
    },
    async connect(isRunning = false) {
      await this._initGameModules();
      setTimeout(() => {
        this._connection = new ZRPWebsocketAdapter(
          Backend.getDynamicUrl(Endpoint.Websocket, { id: (this.gameId ?? -1).toString() }),
          (this.gameId ?? -1).toString()
        );
        const events = useGameEvents();
        this._connection.readMessages(events.handleIncomingEvent);

        useWakeLock().then(lock => {
          if (lock) {
            this._wakeLock = lock;
          }
        });

        if (isRunning) {
          events.pushEvent(ZRPMessageBuilder.build(ZRPOPCode.GameStarted, {}));
        }
      }, 0);
    },
    async tryLeave() {
      Logger.RouterGuard.warn('routing out of active game');
      if (this._connection) {
        Logger.RouterGuard.warn('force closing game connection');
        this.leave();
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
