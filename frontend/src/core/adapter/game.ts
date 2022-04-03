import { defineStore } from 'pinia';
import { Backend, Endpoint } from '../services/api/apiConfig';
import { getBackendErrorTranslation, unwrapBackendError } from '../services/api/errors';
import { GameManagementService } from '../services/api/GameManagement';
import { GameNameValidator } from '../services/validator/gameName';
import { ZRPWebsocketAdapter } from '../services/ws/MessageDistributer';
import { ZRPMessageBuilder } from '../services/zrp/zrpBuilder';
import { ZRPOPCode, ZRPPayload, ZRPRole } from '../services/zrp/zrpTypes';
import { useGameEvents } from './play/events';

let initializedGameModules = false;

export const useGameConfig = defineStore('game-config', {
  state: () => ({
    gameId: -1,
    name: '',
    role: undefined as ZRPRole | undefined,
    inActiveGame: false,
    _connection: undefined as ZRPWebsocketAdapter | undefined
  }),
  getters: {
    host: state => state.role === ZRPRole.Host
  },
  actions: {
    async create(name: string, isPublic: boolean, password: string) {
      const nameValid = new GameNameValidator().validate(name);
      if (!nameValid.isValid) throw nameValid.getErrors();

      const status = await GameManagementService.createGame(name, isPublic, password);
      const [game, error] = unwrapBackendError(status);
      if (error) {
        throw getBackendErrorTranslation(error);
      } else if (game) {
        this.$patch({
          inActiveGame: true,
          role: ZRPRole.Host,
          gameId: game.id,
          name: game.name
        });
        this.connect();
      }
    },
    async join(id: number, password: string, asPlayer: boolean, asSpectator: boolean) {
      if (asPlayer && asSpectator) {
        throw new Error('cant join as player & spectator');
      }

      const status = await GameManagementService.joinGame(id, asPlayer ? ZRPRole.Player : ZRPRole.Spectator, password);
      const [game, error] = unwrapBackendError(status);
      if (error) {
        throw getBackendErrorTranslation(error);
      } else if (game) {
        this.$patch({
          inActiveGame: true,
          role: asPlayer ? ZRPRole.Player : ZRPRole.Spectator,
          gameId: game.id,
          name: game.name
        });
        this.connect();
      }
    },
    async _initGameModules(): Promise<void> {
      if (!initializedGameModules) {
        // TODO: revisit this when working on code splitting
        const _LobbyModule = await import('./play/lobby');
        _LobbyModule.useLobbyStore().__init__();
        initializedGameModules = true;
      }
    },
    async connect() {
      await this._initGameModules();
      setTimeout(() => {
        this._connection = new ZRPWebsocketAdapter(Backend.getDynamicUrl(Endpoint.Websocket, { id: this.gameId.toString() }), this.gameId.toString());
        const events = useGameEvents();
        this._connection.readMessages(events.handleIncomingEvent);
      }, 0);
    },
    async sendEvent<C extends ZRPOPCode>(code: C, payload: ZRPPayload<C>) {
      if (this._connection) {
        this._connection.writeMessage(ZRPMessageBuilder.build(code, payload));
      }
    }
  }
});
